namespace CompactInventory
{
    // Game-free logic. This file must not use any game or BepInEx type, because the unit tests compile it alone.
    public static class SizeRule
    {
        /// <summary>
        /// True when a config item's Size entry must change to 1x1: the list has at least two
        /// values, and both are 1 or more, and it is not already 1x1. An entry with fewer than
        /// two values, or a value below 1, is left unchanged, because the meaning of such a size
        /// in the game config is not known and the mod must not invent one.
        /// </summary>
        public static bool NeedsShrink(int count, int width, int height)
        {
            if (count < 2) return false;
            if (width < 1 || height < 1) return false;
            return width != 1 || height != 1;
        }
    }
}
