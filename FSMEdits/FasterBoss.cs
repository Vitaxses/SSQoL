using PD = PlayerData;

namespace QoL.FSMEdits;

public static class FsmFasterBoss
{
    internal static void FasterBoss(PlayMakerFSM fsm) 
    {
        if (!Configs.FasterBossLoad.Value)
            return;
        
        FasterLace(fsm);
        FasterGMS(fsm);
        FasterTrobbio(fsm);
        FasterWidow(fsm);
        FasterLastJudge(fsm);
        FasterUnravelled(fsm);
        FasterCraggler(fsm);
    }

    private static void FasterCraggler(PlayMakerFSM fsm)
    {
        if (fsm is not { FsmName: "Control", name: "Roof Crab" })
            return;

        fsm.ChangeTransition("Meet?", "ROAR", "Attack Antic");
    }

    private static void FasterUnravelled(PlayMakerFSM fsm)
    {
        if (fsm is not { FsmName: "Control", name: "Boss Scene", gameObject.scene.name: "Ward_02_boss"})
            return;

        Plugin.Logger.LogDebug("Modifying TheUnravelled Boss FSM");

        FsmState state = fsm.GetState("Arena Start")!;
        state.DisableAction(2); // GetPlayerDataBool
        state.InsertMethod(2, (action) => 
        {
            fsm.FindBoolVariable("Boss Encountered")!.RawValue = true;
        });
        state.GetFirstActionOfType<WaitBool>()!.time = 1f;

        fsm.GetState("Encountered Start")!.GetFirstActionOfType<Wait>()!.time = 0.5f;

        fsm.GetState("P2 Pause")!.GetFirstActionOfType<WaitBool>()!.time = 0.5f;
        fsm.GetState("P2 Setup")!.GetFirstActionOfType<WaitBool>()!.time = 1f;

        fsm.GetState("P3 Pause")!.DisableAction(0); // Wait
    }

    internal static void FasterLastJudge(PlayMakerFSM fsm)
    {
        if (fsm.FsmName != "Control" || fsm.gameObject.scene.name != "Coral_Judge_Arena")
            return;
        
        if (fsm.name == "Boss Scene" && PD.instance.bellShrineBellhart && PD.instance.bellShrineBoneForest 
            && PD.instance.bellShrineGreymoor && PD.instance.bellShrineShellwood && PD.instance.bellShrineWilds)
        {
            fsm.ChangeTransition("Init", "UNENCOUNTERED", "Encountered");
            Plugin.Logger.LogDebug("Modifying LastJudge Boss Door FSM");
        }
            
        else if (fsm.name == "Last Judge") {
            Plugin.Logger.LogDebug("Modifying LastJudge Boss FSM");

            fsm.GetState("Intro Roar")!.AddMethod((action) =>
            {
                PD.instance.encounteredLastJudge = true;
            });
        }
    }

    internal static void FasterLace(PlayMakerFSM fsm)
    {
        if (fsm.FsmName != "Control")
            return;
        
        if (fsm.gameObject is { name: "Lace Boss1", scene.name: "Bone_East_12" })
        {
            Plugin.Logger.LogDebug("Modifying Lace1 Boss FSM");

            fsm.InsertAction("Encountered?", new HutongGames.PlayMaker.Actions.SetPlayerDataBool()
            {
                boolName = nameof(PD.encounteredLace1),
                value = true
            }, 0);
        }
        
        else if (fsm.gameObject is { name: "Intro Control", scene.name: "Abyss_Cocoon" })
        {
            Plugin.Logger.LogDebug("Modifying LostLace Boss FSM");

            fsm.ChangeTransition("Check Encountered", FsmEvent.Finished.Name, "Encountered");
        }

        else if (fsm is { name: "door_entry", FsmName: "Control", gameObject.scene.name: "Abyss_Cocoon" })
        {
            PlayerDataBoolTest pdbt = fsm.GetState("Silk Darkness?")!.GetFirstActionOfType<PlayerDataBoolTest>()!;
            pdbt.isFalse = pdbt.isTrue;

            PlayerDataBoolTest pdbt2 = fsm.GetState("Return Control")!.GetFirstActionOfType<PlayerDataBoolTest>()!;
            pdbt2.isFalse = pdbt2.isTrue;
        }
    }

    internal static void FasterGMS(PlayMakerFSM fsm) 
    {
        if (fsm is not { FsmName: "First Challenge", name: "Intro Sequence", gameObject.scene.name: "Cradle_03" })
            return;

        Plugin.Logger.LogDebug("Modifying GMS Boss FSM");
    
        fsm.GetFirstActionOfType<Wait>("Wait For Beat End")!.time = 0.5f;
        fsm.GetFirstActionOfType<AnimatorPlay>("Burst Anim")!.normalizedTime = 5f;
        fsm.DisableActionsOfType<ListenForAnimationEvent>("Burst Anim");
        fsm.GetFirstActionOfType<Wait>("Ready Wait")!.time = 0.2f;
        fsm.GetFirstActionOfType<Wait>("Intro Shake")!.time = 1f;
    }
    
    internal static void FasterTrobbio(PlayMakerFSM fsm)
    {
        if (fsm is { FsmName: "Control", name: "Trobbio" })
        {
            Plugin.Logger.LogDebug("Modifying Trobbio Boss FSM");

            fsm.ChangeTransition("Init", FsmEvent.Finished.Name, "Wait Refight");
            fsm.GetState("Start Pause")!.DisableAction(0); // Wait
            fsm.ChangeTransition("Start Pause", FsmEvent.Finished.Name, "Quick Entrance 1");
        }

        else if (fsm is { FsmName: "Control", name: "Tormented Trobbio" })
        {
            Plugin.Logger.LogDebug("Modifying TormentedTrobbio Boss FSM");
            
            fsm.ChangeTransition("State", "MEET", "Start Pause");
            fsm.GetState("State")!.DisableAction(1); // GetPlayerDataBool
            fsm.GetState("Wait")!.AddMethod((a) =>
            {
                fsm.FindBoolVariable("Encountered")!.Value = true;
            });

            fsm.GetState("Trobbio Rise")!.GetFirstActionOfType<ConvertBoolToFloat>()!.trueValue = 0.2f;
            fsm.GetState("Rise End")!.GetFirstActionOfType<ConvertBoolToFloat>()!.trueValue = 0f;
        }
    }
    
    internal static void FasterWidow(PlayMakerFSM fsm)
    {
        if (fsm is not { FsmName: "Control", name: "Boss Scene", gameObject.scene.name: "Belltown_Shrine" })
            return;
            
        Plugin.Logger.LogDebug("Modifying Widow Boss FSM");

        fsm.ChangeTransition("Check State", "UNENCOUNTERED", "State 1");
        fsm.GetState("Spinner Look")!.DisableAction(2); // Wait
    }
}
