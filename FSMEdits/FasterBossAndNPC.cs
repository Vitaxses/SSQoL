using HutongGames.PlayMaker.Actions;

using Silksong.FsmUtil;

using UnityEngine;

namespace QoL.FSMEdits
{
    internal static class FasterBossAndNpc
    {
        internal static void FasterBoss(PlayMakerFSM fsm)
        {
            if (!QoLPlugin.FasterBossLoad.Value)
                return;

            if (fsm is not { FsmName: "Control", name: "Lace Boss1" })
                return;

            PlayerDataBoolTest? actionPdTest = fsm.Fsm.GetState("Encountered?")!
                .GetFirstActionOfType<PlayerDataBoolTest>();

            if (actionPdTest != null)
                actionPdTest.isFalse = actionPdTest.isTrue;
        }

        internal static void FasterNPC(PlayMakerFSM fsm)
        {
            if (!QoLPlugin.FasterNPC.Value || fsm.FsmName != "Dialogue")
                return;

			if (fsm.gameObject is { name: "Seamstress", scene.name: "Bone_East_Umbrella" }) {
				fsm.GetState("DLG After Dress")!.DisableAction(0);
				fsm.gameObject.transform.Find("Exit Lore Wall").localPosition = new Vector3(-30f, 1.91f, 0f);
			} else if (fsm.gameObject is { name: "Enclave Caretaker FirstMeet", scene.name: "Song_Enclave" }) {
				PlayerData.instance.metCaretaker = true;
			}
		}
    }
}