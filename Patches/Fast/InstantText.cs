namespace QoL.Patches.Fast;

[HarmonyPatch(typeof(DialogueBox), nameof(DialogueBox.Start))]
internal static class DialogueBoxPatch
{
    [HarmonyWrapSafe, HarmonyPostfix]
    private static void Postfix_Start(DialogueBox __instance)
    {
        if (!Configs.InstantText.Value)
            return;

        __instance.currentRevealSpeed = __instance.fastRevealSpeed *= 150;
        __instance.animator.speed = 3f;
    }
}

[HarmonyPatch(typeof(DialogueYesNoBox), nameof(DialogueYesNoBox.Awake))]
internal static class DialogueYesNoBoxPatch
{
    [HarmonyWrapSafe, HarmonyPostfix]
    private static void Postfix_Awake(DialogueYesNoBox __instance)
    {
        if (!Configs.InstantText.Value)
            return;

        __instance.textRevealSpeed = 250f;
        __instance.textRevealWait = 0.1f;
        __instance.animator.speed = 2f;
    }
}
