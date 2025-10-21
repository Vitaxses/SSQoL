using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace QoL.FSMEdits;

internal static class Ventrica
{
    private static bool IsInVentricaScene(Component component) =>
        FastTravelScenes._tubeScenes.ContainsValue(
            GameManager.InternalBaseSceneName(component.gameObject.scene.name)
        );

    internal static void Tube(PlayMakerFSM fsm)
    {
        if (fsm is not { FsmName: "Tube Travel", name: "City Travel Tube" } || !IsInVentricaScene(fsm))
            return;

        Plugin.Logger.LogDebug("Modifying Ventrica FSM");

        if (Configs.FasterVentricaTravel.Value)
        {
            // Fast arrival
            fsm.DisableAction("Tube Start Away", 3);
            fsm.GetAction<SendEventByName>("Tube Start Away", 4)!
                .sendEvent = "START OPEN";
            fsm.ChangeTransition("Start In Tube", FsmEvent.Finished.Name, "Break Items");
            fsm.AddTransition("Break Items", FsmEvent.Finished.Name, "Open");
            fsm.DisableAction("Open", 3);
            fsm.AddTransition("Open", FsmEvent.Finished.Name, "Hop Out Antic");

            // Fast departure
            fsm.ChangeTransition("Preload Scene", FsmEvent.Finished.Name, "Close");
            fsm.AddTransition("Close", FsmEvent.Finished.Name, "Save State");
            fsm.GetAction<ScreenFader>("Fade Out", 2)!.duration = 0.25f;
            fsm.GetAction<Wait>("Fade Out", 3)!.time = 0.25f;
        }

        if (Configs.FasterVentricaBuy.Value)
        {
            fsm.GetAction<SendEventByName>("Unlock Open", 1)!.sendEvent = "START OPEN";
            fsm.AddTransition("Unlock Open", FsmEvent.Finished.Name, "Unlock");
        }
    }

    internal static void Toll(PlayMakerFSM fsm)
    {
        if (!Configs.FasterVentricaBuy.Value)
            return;

        if (fsm is not { FsmName: "Unlock Behaviour", name: "tube_toll_machine" } || !IsInVentricaScene(fsm))
            return;

        Plugin.Logger.LogDebug("Modifying Ventrica Toll FSM");

        fsm.DisableAction("Retract Animation", 0);
        fsm.AddLambdaMethod("Retract Animation", (finish) => {
            fsm.GetComponent<Animator>().speed = 100f;
            finish();
        });
        fsm.DisableAction("After Retract Pause", 1);
    }
}
