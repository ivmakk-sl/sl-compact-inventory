using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using GameCore.HotUpdate;
using GameCore.HotUpdate.Battle.Logic;
using HarmonyLib;

namespace CompactInventory
{
    [BepInPlugin(PluginGuid, "Compact Inventory", "1.0.0")]
    [BepInProcess("SurvivalLog.exe")]
    public sealed class Plugin : BasePlugin
    {
        public const string PluginGuid = "com.ivmakk.survivallog.compactinventory";

        internal static new ManualLogSource Log;
        internal static Harmony Harmony;
        internal static ConfigEntry<bool> Verbose;

        public override void Load()
        {
            Log = base.Log;
            Verbose = Config.Bind(
                "General", "Verbose", false,
                "Log extra detail at Debug level. Keep off in normal play.");
            Harmony = new Harmony(PluginGuid);
            Harmony.PatchAll();
            Log.LogInfo("Compact Inventory loaded.");
        }
    }

    // ConfigManager.Initialize (an async state machine) awaits every LoadConfig<...> call, including
    // Config_Item, before it calls the plain Init* methods in sequence. LoadConfig itself is generic
    // and async, so it cannot be patched directly. InitTowerWaveRewardTiers is the primary hook: it
    // runs after the item config has loaded and before any UI reads it.
    [HarmonyPatch(typeof(ConfigManager), "InitTowerWaveRewardTiers")]
    internal static class ApplyOnConfigInitPatch
    {
        private static void Postfix(ConfigManager __instance)
        {
            try
            {
                int changed = SizeApplier.Apply(__instance);
                SizeApplier.Log("InitTowerWaveRewardTiers", changed);
            }
            catch (Exception e)
            {
                Plugin.Log.LogError($"Compact Inventory: apply at InitTowerWaveRewardTiers failed: {e}");
            }
        }
    }

    // Guard for a dictionary rebuilt after the config-load hook ran (for example a language switch):
    // AddItem reads Config_Item.Size for each saved item, so the fix must be back in place before this
    // creates the saved items.
    [HarmonyPatch(typeof(ItemManager), "LoadItemsFromSave")]
    internal static class ApplyBeforeLoadItemsFromSavePatch
    {
        private static void Prefix()
        {
            try
            {
                int changed = SizeApplier.Apply(ConfigManager.Instance);
                SizeApplier.Log("LoadItemsFromSave", changed);
            }
            catch (Exception e)
            {
                Plugin.Log.LogError($"Compact Inventory: apply at LoadItemsFromSave failed: {e}");
            }
        }
    }

    // Idempotent apply step, shared by both hooks. Mutates Config_Item.Size in place so every reader
    // of the config (item creation, save load, and each UI reader) sees 1x1, per design.md.
    internal static class SizeApplier
    {
        private static bool loggedFirst;

        // Walks the item config dictionary and shrinks an oversized Size to 1x1 in place. Returns the
        // number of entries changed. Skips with no error when the manager or its item dictionary is
        // not ready yet, so a hook that fires early cannot break config load.
        public static int Apply(ConfigManager mgr)
        {
            if (mgr == null) return 0;
            var items = mgr._Config_Item_Dict;
            if (items == null) return 0;

            int changed = 0;
            var e = items.GetEnumerator();
            while (e.MoveNext())
            {
                var config = e.Current.Value;
                var size = config?.Size;
                if (size == null) continue;
                int width = size.Count >= 2 ? size[0] : 0;
                int height = size.Count >= 2 ? size[1] : 0;
                if (!SizeRule.NeedsShrink(size.Count, width, height)) continue;
                size[0] = 1;
                size[1] = 1;
                changed++;
            }
            return changed;
        }

        // Every line is Debug behind Verbose. The first apply that actually changes an entry gets
        // a distinct message with the changed count; every other call (nothing to change, or a
        // later hook) logs a plain per-hook line.
        public static void Log(string hook, int changed)
        {
            if (!Plugin.Verbose.Value) return;

            if (changed > 0 && !loggedFirst)
            {
                loggedFirst = true;
                Plugin.Log.LogDebug($"Set {changed} item sizes to 1x1 ({hook})");
            }
            else
            {
                Plugin.Log.LogDebug($"Compact Inventory: {hook} apply, {changed} changed");
            }
        }
    }
}
