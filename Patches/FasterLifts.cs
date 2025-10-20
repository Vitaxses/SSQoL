namespace QoL.Patches;

[HarmonyPatch(typeof(LiftControl), nameof(LiftControl.Start))]
internal static class LiftControlPatch
{
    [HarmonyWrapSafe, HarmonyPostfix]
    private static void Postfix_Start(LiftControl __instance)
    {
        string sceneName = __instance.gameObject.scene.name;
        if (!Configs.FasterLifts.Value || sceneName == "Ward_01")
            return;

        __instance.moveSpeed = sceneName == "Library_11" ? 25f : 150f;
        __instance.moveDelay = 0f;
        __instance.endDelay = 0f;
    }
}
