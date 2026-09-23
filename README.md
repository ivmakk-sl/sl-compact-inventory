# Compact Inventory

A mod for the Steam game *Survival Log* that makes each item take one cell (1x1) in every bag grid: the character bag, each storage, and every other panel that shows a bag grid. A large item such as Whole Lamb or Rack of Ribs takes the same one cell as a small item, so you manage inventory by item count and weight, not by shape.

Items already in an existing save also become 1x1 when you load that save with the mod installed. The mod changes item grid size; it does not change item weight, stack limits, bag sizes, or trade value.

## Requirements

The [BepInEx Pack for Survival Log](https://www.nexusmods.com/survivallog/mods/12), the BepInEx 6 (IL2CPP) build for the game.

## Install

1. Install the [BepInEx Pack for Survival Log](https://www.nexusmods.com/survivallog/mods/12) (if no other mods were installed before, start the game once so BepInEx finishes setup, then quit).
2. Extract this mod's zip into the game folder (the folder with the game .exe). The DLL lands in `BepInEx\plugins`. Full path example:
   - Steam: `C:\Program Files (x86)\Steam\steamapps\common\Survival Log\BepInEx\plugins\CompactInventory.dll`

## Uninstall

Delete `CompactInventory.dll` from the `BepInEx\plugins` folder.

Before uninstalling, back up your save and spread out or remove items from crowded bags, then save. Items regain their normal size on the next load. The game may rearrange items or drop some beside storage. Check the ground afterward. Making room may help, but is not guaranteed to prevent this.

## Build

This is a BepInEx 6 IL2CPP plugin. It compiles against the game's IL2CPP interop assemblies, so a game install with BepInEx set up and started once is required. Those assemblies are game-derived and are not part of this repo. The .NET 8 SDK is required.

```
dotnet build src/CompactInventory.csproj -c Release
```

`Directory.Build.props` sets `GameDir` to the default Steam install path. If the game is in another place, override it without an edit of the file: set a `GameDir` environment variable, or pass `-p:GameDir=...` on the build. The output DLL is at `src\bin\Release\CompactInventory.dll`.

The size rule that decides which item config entries change is game-free code (`src/SizeRule.cs`) with unit tests. The tests do not need the game:

```
dotnet test tests/CompactInventory.Tests
```

## Package

Add `-p:Package=true` to a Release build to also write the ready-to-install zip at `dist\CompactInventory-<version>.zip`, laid out as `BepInEx\plugins\CompactInventory.dll` so a user extracts it at the game root. A plain build skips this step.

```
dotnet build src/CompactInventory.csproj -c Release -p:Package=true
```

## License

Licensed under the GNU General Public License v3.0. Copyright (C) 2026 ivmakk. See [LICENSE](LICENSE).

You may reuse and modify this mod, but you must keep it open under the same license and give credit. Do not reupload it without credit.
