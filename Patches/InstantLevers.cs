using HarmonyLib;

namespace QoL.Patches
{
    [HarmonyPatch(typeof(Lever))]
    internal class LeverPatch
    {
        [HarmonyPatch(typeof(Lever), nameof(Lever.Start))]
        [HarmonyPostfix]
        static void LeverStartPostfix(Lever __instance)
        {
            if (QoLPlugin.InstantLevers.Value) __instance.openGateDelay = 0f;
        }
    }

    [HarmonyPatch(typeof(Lever_tk2d))]
    internal class Lever_tk2dPatch
    {

        [HarmonyPatch(typeof(Lever_tk2d), nameof(Lever_tk2d.Start))]
        [HarmonyPostfix]
        static void TK2DLeverStartPostfix(Lever_tk2d __instance)
        {
            if (QoLPlugin.InstantLevers.Value) __instance.openGateDelay = 0f;

        }
    }

    [HarmonyPatch(typeof(PressurePlateBase))]
    internal class PressurePlateBasePatch
    {

        [HarmonyPatch(typeof(PressurePlateBase), nameof(PressurePlateBase.Awake))]
        [HarmonyPostfix]
        static void TK2DLeverStartPostfix(PressurePlateBase __instance)
        {
            if (QoLPlugin.InstantLevers.Value) __instance.gateOpenDelay = __instance.waitTime = __instance.dropTime = 0f;
        }
    }
}