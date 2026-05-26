namespace QoL.Patches;

[HarmonyPatch(typeof(Lever), nameof(Lever.Start))]
internal static class LeverPatch
{
    [HarmonyWrapSafe, HarmonyPostfix]
    private static void Postfix_Start(Lever __instance)
    {
        if (Configs.InstantLevers.Value)
            __instance.openGateDelay = 0f;

        if (Configs.LeverSkips.Value && __instance.gameObject.name.StartsWith("Understore Lever") && GameManager.instance.sceneName == "Ward_01" )
        {
            __instance.canHitTrigger = null;
        }
    }
}

[HarmonyPatch(typeof(Trapdoor), nameof(Trapdoor.Awake))]
internal static class TrapdoorPatch
{
    [HarmonyWrapSafe, HarmonyPostfix]
    private static void Postfix_Awake(Trapdoor __instance)
    {
        if (Configs.InstantLevers.Value)
        {
            __instance.retractStartDelay = __instance.retractEndDelay = __instance.openWaitTime = 0f;
        }
    }
}


[HarmonyPatch(typeof(TrapBridgeExtend), nameof(TrapBridgeExtend.Awake))]
internal static class TrapBridgeExtendPatch
{
    [HarmonyWrapSafe, HarmonyPostfix]
    private static void Postfix_Awake(TrapBridgeExtend __instance)
    {
        if (Configs.InstantLevers.Value)
        {
            __instance.animator.speed = 8f;
            __instance.openDelay = __instance.anticDelay = 0f;
        }
    }
}

[HarmonyPatch(typeof(Lever_tk2d), nameof(Lever_tk2d.Start))]
internal static class Lever_tk2dPatch
{
    [HarmonyWrapSafe, HarmonyPostfix]
    private static void Postfix_Start(Lever_tk2d __instance)
    {
        if (Configs.InstantLevers.Value)
            __instance.openGateDelay = 0f;

        if (Configs.LeverSkips.Value && __instance.gameObject is { name: "Song_lever_side", scene.name: "Bone_10" })
        {
            __instance.transform.GetChild(2).transform.position = new (96.4f, 30.11f, 0.005f);
        }
    }
}

[HarmonyPatch(typeof(Gate), nameof(Gate.Start))]
internal static class GatePatch
{
    [HarmonyWrapSafe, HarmonyPostfix]
    private static void Postfix_Start(Gate __instance)
    {
        if (Configs.OldPutrifiedPlanks.Value && __instance.gameObject is { name: "drop_planks", scene.name: "Aqueduct_04" })
        {
            var canHitCollidder = __instance.transform.GetChild(3).GetComponent<BoxCollider2D>();
            canHitCollidder.size = new(canHitCollidder.size.x * 2, 50f);
        }
    }
}

[HarmonyPatch(typeof(PressurePlateBase), nameof(PressurePlateBase.Awake))]
internal static class PressurePlateBasePatch
{
    [HarmonyWrapSafe, HarmonyPostfix]
    private static void Postfix_Start(PressurePlateBase __instance)
    {
        if (!Configs.InstantLevers.Value)
            return;

        __instance.gateOpenDelay = 0f;
        __instance.dropTime = __instance.waitTime = 0.05f;
    }
}

[HarmonyPatch(typeof(DialDoorBridge), nameof(DialDoorBridge.Start))]
internal static class DialDoorBridgePatch
{
    [HarmonyWrapSafe, HarmonyPostfix]
    private static void Postfix_Start(DialDoorBridge __instance)
    {
        if (Configs.InstantLevers.Value)
        {
            __instance.doorOpenDelay = __instance.moveDelay = __instance.moveDuration = 0f;
        }        
    }
}
