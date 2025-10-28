namespace QoL.Patches;

[HarmonyPatch(typeof(UIManager), nameof(UIManager.Start))]
internal static class UIManagerPatch
{
    [HarmonyWrapSafe, HarmonyPostfix]
    private static void Postfix(UIManager __instance)
    {
        if (Configs.FastUI.Value)
            __instance.MENU_FADE_SPEED = 100;
    }
}
