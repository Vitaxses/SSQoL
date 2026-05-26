namespace QoL.Patches.Fast;

[HarmonyPatch(typeof(UIManager), nameof(UIManager.Start))]
internal static class UIManagerPatch
{
    [HarmonyWrapSafe, HarmonyPostfix]
    private static void Postfix_Start()
    {
        Configs.FastUI.SettingChanged += (sender, e) =>
        {
            Adjust();
        };

        Adjust();
    }

    internal static void Adjust()
    {
        if (Configs.FastUI.Value) UIManager.instance.MENU_FADE_SPEED = 13f;
        else UIManager.instance.MENU_FADE_SPEED = 3.2f; // default
    }
}
