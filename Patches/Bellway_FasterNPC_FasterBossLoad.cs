using HarmonyLib;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Silksong.FsmUtil;

namespace QoL.Patches
{
    [HarmonyPatch(typeof(PlayMakerFSM), nameof(PlayMakerFSM.Start))]
    internal class PlayMakerFSMPatch
    {

        [HarmonyPostfix]
        static void Postfix(PlayMakerFSM __instance)
        {
            Bellway(__instance);
            FasterNPC(__instance);
            FasterBoss(__instance);
        }

        private static void FasterBoss(PlayMakerFSM Fsm_Comp)
        {
            if (!QoLPlugin.FasterBossLoad.Value) return;

            FsmState EncounteredState = null;

            if (Fsm_Comp.FsmName.Equals("Control"))
            {
                string GOName = Fsm_Comp.gameObject.name;

                if (GOName.Equals("Lace Boss1"))
                {
                    EncounteredState = Fsm_Comp.Fsm.GetState("Encountered?");
                }
            }

            if (EncounteredState == null) return;

            PlayerDataBoolTest PlayerDataTestAction = EncounteredState.GetFirstActionOfType<PlayerDataBoolTest>();

            if (PlayerDataTestAction != null) PlayerDataTestAction.isFalse = PlayerDataTestAction.isTrue;
        }

        private static void Bellway(PlayMakerFSM Fsm)
        {
            if (!QoLPlugin.BellBeastFreeWill.Value && !QoLPlugin.FasterBellwayBuy.Value && !QoLPlugin.NoBellBeastSleep.Value) return;

            if (QoLPlugin.FasterBellwayBuy.Value && Fsm.FsmName.Equals("Unlock Behaviour"))
            {
                FsmState state1 = Fsm.Fsm.GetState("Return Control");
                if (state1 == null) return;
                FsmState state2 = Fsm.Fsm.GetState("Allow Bellbeast Call");
                if (state2 == null) return;
                var transition = state1.GetTransition(0);
                transition.ToState = "Allow Bellbeast Call";
                transition.toFsmState = state2;
                state2.AddLambdaMethod((action) => { Fsm.Fsm.SetState("Inert"); });
                return;
            }

            if (!Fsm.FsmName.Equals("Interaction") || !Fsm.gameObject.name.Equals("Bone Beast NPC")) return;
            
            FsmState SleepChoiceState = Fsm.Fsm.GetState("Start State");
            if (SleepChoiceState == null) return;

            if (QoLPlugin.BellBeastFreeWill.Value)
            {
                FsmState IsHereState = Fsm.Fsm.GetState("Is Already Present?");
                if (IsHereState == null) return;

                FsmTransition transition = IsHereState.GetTransition(0);
                transition.toState = "Start State";
                transition.toFsmState = SleepChoiceState;
            }

            if (QoLPlugin.NoBellBeastSleep.Value && SleepChoiceState != null)
            {
                SleepChoiceState.RemoveFirstActionOfType<SendRandomEvent>();
                SendEvent sendEventAction = SleepChoiceState.GetFirstActionOfType<SendEvent>();
                if (sendEventAction == null) return;
                sendEventAction.Enabled = true;
                sendEventAction.sendEvent = Fsm.Fsm.FindEvent("WAKE");
            }
        
        }
        
        private static void FasterNPC(PlayMakerFSM Fsm)
        {
            if (!QoLPlugin.FasterNPC.Value || Fsm.FsmName != "Dialogue") return;

            if (Fsm.gameObject.scene.name.Equals("Bone_East_Umbrella"))
            {
                FsmState state = Fsm.Fsm.GetState("DLG After Dress");
                if (state == null) return;
                SetBoolValue action = state.GetFirstActionOfType<SetBoolValue>();
                if (action != null) action.boolValue = false;
                Fsm.gameObject.transform.GetChild(5).localPosition = new UnityEngine.Vector3(-30f, 1.91f, 0f);
            }
            else if (Fsm.gameObject.scene.name.Equals("Song_Enclave") && Fsm.gameObject.name.Equals("Enclave Caretaker FirstMeet"))
            {
                PlayerData.instance.metCaretaker = true;
            }
        }

    }
}