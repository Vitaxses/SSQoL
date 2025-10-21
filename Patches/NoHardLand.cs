namespace QoL.Patches;

[HarmonyPatch(typeof(HeroController), nameof(HeroController.ShouldHardLand))]
internal static class HardLandPatch
{
    [HarmonyWrapSafe, HarmonyPostfix]
    private static void ShouldHardLandPrefix(ref bool __result)
    {
        if (Configs.NoHardFalls.Value)
            __result = false;
    }
}
