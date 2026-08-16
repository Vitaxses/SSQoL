namespace QoL.Patches.Fast;

[HarmonyPatch(typeof(DialogueBox))]
internal static class DialogueBoxPatch
{
    [HarmonyPatch(nameof(DialogueBox.Start))]
    [HarmonyWrapSafe, HarmonyPostfix]
    private static void Postfix_Start(DialogueBox __instance)
    {
        if (!Configs.InstantText.Value)
            return;

        __instance.currentRevealSpeed = __instance.fastRevealSpeed *= 150;
        // __instance.animator.speed = 3f;
        __instance.lineEndPause = __instance.firstOpenDelay = 0f;
    }

    
    [HarmonyPatch(nameof(DialogueBox.Update))]
    [HarmonyWrapSafe, HarmonyPostfix]
    private static void Postfix_Update(DialogueBox __instance)
    {
        if (!Configs.InstantText.Value)
            return;

        if (ManagerSingleton<InputHandler>.Instance.WasSkipButtonPressed)
		{
			__instance.AdvanceConversation();
		}
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
