using HutongGames.PlayMaker.Actions;

namespace QoL.FSMEdits;

internal static class Cutscene
{
    internal static void LastDive(PlayMakerFSM fsm)
    {
        if (!Configs.SkipCutscene.Value)
            return;

        if (fsm is not { FsmName: "Dive Cutscene", gameObject: { name: "Abyss Dive Cutscene", scene.name: "Abyss_05" } })
            return;

        fsm.DisableAction("Preload Scene", 3); // Don't Preload
        BeginSceneTransition bst = fsm.GetAction<BeginSceneTransition>("Dive End", 9)!;
        bst.sceneName = "Abyss_Cocoon";
        bst.entryGateName = "door_entry";
    }
}
