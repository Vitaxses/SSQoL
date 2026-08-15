using QoL.FSMEdits;

namespace QoL.Patches;

[HarmonyPatch(typeof(PlayMakerFSM), nameof(PlayMakerFSM.Start))]
internal static class PlayMakerFSMPatch
{
    // Note: put all fsm edit methods here
    // Feel free to ignore nullity checks as they will be caught here
    private static readonly Action<PlayMakerFSM>[] edits =
    [
        FsmFasterBoss.FasterBoss,
        FasterNpc.FasterNPC,
        Bellway.BellBeast,
        Bellway.Toll,
        Ventrica.Tube,
        Ventrica.Toll,
        FsmLiftControl.Lift,
        BeastlingCall.SilkSpecials,
        BeastlingCall.Beastlings,
        BeastlingCall.Needolin,
        FsmCutscene.Pinstress,
        FsmCutscene.Doctor,
        FsmCutscene.Plinney,
        FsmCutscene.LastDive,
        FsmCutscene.Lace2,
        FsmCutscene.FleaTravel,
        FsmCutscene.ShermaAct3Intro,
        FsmCutscene.DivingBell,
        FsmCutscene.CitadelEntrance,
        DreamCutscene.Widow,
        DreamCutscene.FirstSinner,
        DreamCutscene.MemoryThread,
        DreamCutscene.SilkHeart,
        ToolPatch.Scuttlebrace,
        Abilities.NeedleStrike,
        Abilities.Bind,
        Abilities.FaydownNeedolinCheck,
        SkipWeakness.SkipWeaknessPatch,
        FastMenu.ShopUI,
        FastMenu.QuestUIPrompt,
    ];

    [HarmonyPostfix]
    private static void Postfix_Start(PlayMakerFSM __instance)
    {
        foreach (Action<PlayMakerFSM> edit in edits)
        {
            try
            {
                edit.Invoke(__instance);
            }
            catch (Exception e)
            {
                Plugin.Logger.LogError($"Exception thrown when editing FSM {__instance.FsmName} on {__instance.name}");
                Plugin.Logger.LogError(e);
            }
        }
    }
}
