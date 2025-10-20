using HarmonyLib;

using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

using Silksong.FsmUtil;

using UnityEngine;

namespace QoL.FSMEdits
{
    internal static class Bellway
    {
        internal static bool IsInBellwayScene(Component component) =>
            FastTravelScenes._scenes.ContainsValue(
                GameManager.InternalBaseSceneName(component.gameObject.scene.name)
            );

        internal static void BellBeast(PlayMakerFSM fsm)
        {
            if (!QoLPlugin.BellBeastFreeWill.Value && !QoLPlugin.NoBellBeastSleep.Value)
                return;

            if (fsm is not { FsmName: "Interaction", name: "Bone Beast NPC" } || !IsInBellwayScene(fsm))
                return;
            
            FsmState SleepChoiceState = fsm.Fsm.GetState("Start State");
            if (SleepChoiceState == null) return;

            if (QoLPlugin.BellBeastFreeWill.Value)
            {
                FsmState IsHereState = fsm.Fsm.GetState("Is Already Present?");
                if (IsHereState == null) return;

                FsmTransition transition = IsHereState.GetTransition(0);
                transition.toState = "Start State";
                transition.toFsmState = SleepChoiceState;
            }

            if (QoLPlugin.NoBellBeastSleep.Value && SleepChoiceState != null)
            {
                SleepChoiceState.RemoveFirstActionOfType<SendRandomEvent>();
                SendEvent? sendEventAction = SleepChoiceState.GetFirstActionOfType<SendEvent>();
                if (sendEventAction == null) return;
                sendEventAction.Enabled = true;
                sendEventAction.sendEvent = fsm.Fsm.FindEvent("WAKE");
            }
        
        }

        internal static void Toll(PlayMakerFSM fsm)
        {
            if (!QoLPlugin.FasterBellwayBuy.Value)
                return;

            // Grand Bellway has "Bellway Toll Machine(1)"
            if (fsm is not { FsmName: "Unlock Behaviour" } || !fsm.name.StartsWith("Bellway Toll Machine") || !IsInBellwayScene(fsm))
                return;

            FsmState state1 = fsm.Fsm.GetState("Return Control");
            if (state1 == null)
                return;
            FsmState state2 = fsm.Fsm.GetState("Allow Bellbeast Call");
            if (state2 == null)
                return;
            var transition = state1.GetTransition(0);
            transition.ToState = "Allow Bellbeast Call";
            transition.toFsmState = state2;
            state2.AddLambdaMethod((action) => { fsm.Fsm.SetState("Inert"); });
            return;
        }
    }
}