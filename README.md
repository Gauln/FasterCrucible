# Faster Crucible

A small **server-side** mod for [Vintage Story](https://www.vintagestory.at/) that speeds up
metal smelting in the **crucible**.

## The problem

In Vintage Story the crucible's smelting time grows with the amount of ore inside it. The game
computes the total duration as, summed over every ore stack:

```
duration += perUnitDuration * StackSize / SmeltedRatio
```

So a full crucible — for example 160 copper nuggets (4 stacks of 40, yielding 8 ingots) — takes
roughly **160× as long** as smelting a single nugget. Heating the crucible up to temperature is
quick, but the actual melt-down of a big batch drags on.

## What this mod does

It applies a Harmony **postfix** to `BlockSmeltingContainer.GetMeltingDuration` (the crucible) and
divides the resulting duration by **5**. The whole smelt simply finishes proportionally faster.

- Only the **crucible** is affected. Food cooking, clay firing, ore roasting, and the heat-up phase
  are all left untouched.
- Nothing is re-implemented — the mod only scales the final number — so there is no risk of the
  null-reference crashes that a full method rewrite can cause.
- **Server-side**: install it on the server (or in single-player, which runs an internal server).
  Clients do not need it.

## Speed

The crucible smelting duration is divided by **5** (five times faster). This value is fixed.

## Installation

1. Download the latest `FasterCrucible.zip` from the
   [Releases](https://github.com/Gauln/FasterCrucible/releases) page (or from ModDB).
2. Copy the **zip** (do not unzip it) into your `VintagestoryData/Mods` folder.
3. Restart the game/server.

Confirm it loaded by looking for `[FasterCrucible] OK` in `Logs/server-main.log`.

## Building from source

Requires the .NET 10 SDK and a Vintage Story installation. Set `VintageStoryDir` in
`FasterCrucible.csproj` (or as an environment variable) to your game folder, then:

```
dotnet build -c Release
```

The packaged mod zip is produced at `bin/FasterCrucible.zip`.

## Credits

By **Gauln**, built with [Claude Code](https://claude.com/claude-code). Licensed under the MIT
License — see [LICENSE](LICENSE).
