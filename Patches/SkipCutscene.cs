namespace QoL.Patches;

[HarmonyPatch(typeof(InputHandler), nameof(InputHandler.SetSkipMode))]
internal static class InputHandlerPatch
{
    private static readonly string[] UnskipScene = { "Bone_East_Umbrella", "Belltown", "Room_Pinstress", "Belltown_Room_pinsmith", "Belltown_Room_doctor", "End_Credits_Scroll", "End_Credits", "Menu_Credits", "End_Game_Completion", "PermaDeath", "Bellway_City", "City_Lace_cutscene" };

    [HarmonyWrapSafe, HarmonyPrefix]
    private static bool Prefix(InputHandler __instance, ref GlobalEnums.SkipPromptMode newMode)
    {
        if (!Configs.SkipCutscene.Value || UnskipScene.Contains(GameManager.instance.sceneName)) 
            return true;

        __instance.SkipMode = newMode = GlobalEnums.SkipPromptMode.SKIP_INSTANT;
        __instance.skipCooldownTime = 0f;
        __instance.readyToSkipCutscene = true;
        return false;
    }
}

[HarmonyPatch(typeof(SkippableSequence), nameof(SkippableSequence.CanSkip), MethodType.Getter)]
internal static class SkippableSequencePatch
{
    [HarmonyWrapSafe, HarmonyPrefix]
    private static bool Prefix(SkippableSequence __instance, ref bool __result)
    {
        if (!Configs.SkipCutscene.Value)
            return true;
        
        __instance.AllowSkip();
        __instance.canSkip = __result = true;
        return false;
    }
}

// Skips the Team Cherry icon
[HarmonyPatch(typeof(StartManager), nameof(StartManager.Start))]
internal static class StartManagerPatch
{

    [HarmonyWrapSafe, HarmonyPrefix]
    static void Prefix_Start(StartManager __instance)
    {
        if (Configs.SkipCutscene.Value)
            __instance.startManagerAnimator?.speed = 1000f;
    }
}
