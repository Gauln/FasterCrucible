using System.Reflection;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

[assembly: ModInfo("Faster Crucible", "fastercrucible")]

namespace FasterCrucible
{
    /// <summary>
    /// Entry point. Applies a Harmony postfix that scales down the crucible's smelting
    /// duration by a fixed multiplier. Server-side only (smelting is simulated on the server).
    /// </summary>
    public class FasterCrucibleMod : ModSystem
    {
        private const string HarmonyId = "fastercrucible";

        /// <summary>
        /// The crucible smelting duration is divided by this value.
        /// 1 = vanilla speed, 5 = five times faster. Fixed at build time (not user-configurable).
        /// </summary>
        public const float Multiplier = 5f;

        private Harmony? harmony;

        public override bool ShouldLoad(EnumAppSide forSide) => forSide == EnumAppSide.Server;

        public override void Start(ICoreAPI api)
        {
            base.Start(api);

            harmony = new Harmony(HarmonyId);
            harmony.PatchAll(typeof(FasterCrucibleMod).Assembly);

            // Confirm the target method still exists so the log clearly states whether the patch can apply.
            MethodInfo? target = AccessTools.Method(
                typeof(BlockSmeltingContainer), nameof(BlockSmeltingContainer.GetMeltingDuration));

            if (target != null)
            {
                Mod.Logger.Notification(
                    "[FasterCrucible] OK: crucible smelting duration divided by {0:0.##}x " +
                    "(BlockSmeltingContainer.GetMeltingDuration patched).", Multiplier);
            }
            else
            {
                Mod.Logger.Error(
                    "[FasterCrucible] FAILED: BlockSmeltingContainer.GetMeltingDuration not found. " +
                    "The game's crucible code may have changed. Smelting speed is UNCHANGED.");
            }
        }

        public override void Dispose()
        {
            harmony?.UnpatchAll(HarmonyId);
            base.Dispose();
        }
    }

    /// <summary>
    /// Postfix on <see cref="BlockSmeltingContainer.GetMeltingDuration"/> — the crucible.
    ///
    /// Vanilla computes the total smelting time as the sum, over every ore stack in the
    /// crucible, of (per-unit duration × StackSize / SmeltedRatio). That makes a full
    /// crucible (e.g. 160 nuggets = 4 stacks of 40) take roughly 160× as long as a single
    /// nugget. We simply divide the final result by the fixed multiplier, so the whole
    /// smelt finishes proportionally faster. Nothing is re-implemented, so there is no risk
    /// of null-reference errors, and only crucibles (this class) are affected — food cooking,
    /// clay firing, etc. go through different code and are untouched.
    /// </summary>
    [HarmonyPatch(typeof(BlockSmeltingContainer), nameof(BlockSmeltingContainer.GetMeltingDuration))]
    internal static class Patch_BlockSmeltingContainer_GetMeltingDuration
    {
        [HarmonyPostfix]
        static void Postfix(ref float __result)
        {
            __result /= FasterCrucibleMod.Multiplier;
        }
    }
}
