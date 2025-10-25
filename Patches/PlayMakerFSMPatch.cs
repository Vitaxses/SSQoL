using QoL.FSMEdits;

namespace QoL.Patches;

[HarmonyPatch(typeof(PlayMakerFSM), nameof(PlayMakerFSM.Start))]
internal static class PlayMakerFSMPatch
{
    // Note: put all fsm edit methods here
    // Feel free to ignore nullity checks as they will be caught here
    private static readonly Action<PlayMakerFSM>[] edits =
    [
        FasterBossAndNpc.FasterBoss,
        FasterBossAndNpc.FasterNPC,
        Bellway.BellBeast,
        Bellway.Toll,
        Ventrica.Tube,
        Ventrica.Toll,
        FsmLiftControl.Lift,
        BeastlingCall.SilkSpecials,
        BeastlingCall.Beastlings,
        BeastlingCall.Needolin
    ];

    [HarmonyPostfix]
    private static void Postfix(PlayMakerFSM __instance)
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
