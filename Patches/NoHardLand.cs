using HarmonyLib;

namespace QoL.Patches
{
    [HarmonyPatch(typeof(HeroController), nameof(HeroController.ShouldHardLand))]
    class HardLandPatch
    {

        [HarmonyPrefix]
        static bool ShouldHardLandPrefix(ref bool __result)
        {
            if (QoLPlugin.NoHardFalls.Value) return __result = false;
            return true;
        }
    }
}