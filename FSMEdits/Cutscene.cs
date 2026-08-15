namespace QoL.FSMEdits;

internal static class FsmCutscene
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

    internal static void Pinstress(PlayMakerFSM fsm)
    {
        if (!Configs.SkipCutscene.Value)
            return;

        if (fsm is not { name: "Pinstress Interior Ground Sit", FsmName: "Behaviour" })
            return;

        fsm.ChangeTransition("Reposition", FsmEvent.Finished.Name, "Msg");
        fsm.GetState("Fade Up")?.GetFirstActionOfType<Wait>()?.time = 1f;
    }
    
    internal static void Doctor(PlayMakerFSM fsm)
    {
        if (!Configs.SkipCutscene.Value)
            return;

        if (fsm is not { name: "Doctor Fly", FsmName: "Dialogue" })
            return;

        var cinematicState = fsm.GetState("Cinematic");
        cinematicState?.DisableActionsOfType<StartCinematic>();
        
        var screenFader = cinematicState?.GetFirstActionOfType<ScreenFader>();
        screenFader?.endColour.Value = screenFader.endColour.Value.SetAlpha(1f);
        screenFader?.duration = 1f;

        cinematicState?.AddAction(new Wait()
        {
            time = 1f,
            finishEvent = cinematicState.GetTransition(0).FsmEvent
        });
    }
    
    internal static void Seamstress(PlayMakerFSM fsm)
    {
        if (!Configs.SkipCutscene.Value)
            return;

        if (fsm is not { name: "Seamstress", FsmName: "Dialogue" })
            return;

        var cinematicState = fsm.GetState("Cinematic");
        cinematicState?.DisableActionsOfType<StartCinematic>();
        
        var screenFader = cinematicState?.GetFirstActionOfType<ScreenFader>();
        screenFader?.endColour.Value = screenFader.endColour.Value.SetAlpha(1f);
        screenFader?.duration = 1f;

        cinematicState?.AddAction(new Wait()
        {
            time = 0.5f,
            finishEvent = cinematicState.GetTransition(0).FsmEvent
        });
    }

    internal static void Plinney(PlayMakerFSM fsm)
    {        
        if (!Configs.SkipCutscene.Value)
            return;

        if (fsm is not { name: "Plinney Inside", FsmName: "Dialogue" })
            return;

        fsm.InsertAction("Fade Down", new Wait()
        {
            time = 0f,
            finishEvent = FsmEvent.Finished
        }, 2);

        fsm.ChangeTransition("Fade Down", FsmEvent.Finished.Name, "Upgrade");
    }

    internal static void Lace2(PlayMakerFSM fsm)
    {
        if (!Configs.SkipCutscene.Value)
            return;

        if (fsm is { name: "door_cutsceneEndLaceTower", FsmName: "Travel Control" })
        {
            fsm.ChangeTransition("Lift Arrive?", "FALSE", "Lift Already Here");
            fsm.transform.GetChild(2).GetComponent<Animator>().speed = 5f; // Birdcage
            fsm.GetState("Enter")!.GetFirstActionOfType<iTweenMoveTo>()!.speed = 25f;

            var inLiftState = fsm.GetState("In Lift")!;
            inLiftState.GetAction<Wait>(8)!.enabled = false;

            inLiftState.GetAction<Wait>(10)!.time = inLiftState.GetAction<ScreenFader>(11)!.duration
                = inLiftState.GetAction<Wait>(12)!.time = 0.5f;

            if (PlayerData.instance.encounteredLaceTower)
            {
                fsm.GetFirstActionOfType<SendEventToRegister>("Lift Already Here")!.eventName = "BATTLE START REFIGHT";
            }

            fsm.InsertMethod("Lift Already Here", 0, _ => // Fix audio cutting out
            {
                GameManager.instance.actorSnapshotUnpaused.TransitionToSafe(0f);
			    GameManager.instance.ui.AudioGoToGameplay(0.2f);
            });

            fsm.GetState("Hero Control")!.GetFirstActionOfType<PlayerDataVariableTest>()!.Enabled = PlayerData.instance.laceTowerDoorOpened;
        }
        
        else if (fsm is { FsmName: "Sequence", name: "Boss Scene", gameObject.scene.name: "Cog_Dancers"})
        {
            var coreTransform = fsm.transform.GetChild(12);
            coreTransform.GetComponent<Animator>().speed = 4f; // Core Rotator
            fsm.DisableActionsOfType<Wait>("Lift Inspect Wait");

            coreTransform.GetChild(1).GetChild(2).GetChild(9).GetComponent<Animator>().speed = 5f; // Birdcage

            fsm.GetState("Get In Lift Start")!.InsertMethod((a) =>
            {
                var inspect = fsm.transform.GetChild(5).gameObject;
                inspect.GetComponent<PlayMakerNPC>().Deactivate(false);
                inspect.SetActive(false);
            }, 8);

            fsm.GetState("In Lift")!.GetLastActionOfType<Wait>()!.time = 0.5f;

            fsm.GetState("Get In Lift Move")!.GetFirstActionOfType<iTweenMoveTo>()!.speed = fsm.GetState("Hero Exit")!.GetFirstActionOfType<iTweenMoveTo>()!.speed = 15f;

            fsm.GetState("Fade Down")!.GetFirstActionOfType<Wait>()!.time = 0.7f;

            FsmState exitedState = fsm.GetState("Hero Exited")!;
            var playAnimationWaitAction = exitedState.GetFirstActionOfType<Tk2dPlayAnimationWait>()!;
            playAnimationWaitAction.Enabled = false;
        }
    }

    internal static void FleaTravel(PlayMakerFSM fsm)
    {
        if (!Configs.SkipCutscene.Value)
            return;
        
        if (fsm is { FsmName: "Quest End Sequence", name: "Caravan"} )
        {
            fsm.GetState("Fade Down")!.GetFirstActionOfType<Wait>()!.time = 1.5f;
            
            FsmState travelState = fsm.GetState("Sound With")!;
            travelState.GetFirstActionOfType<StartPreloadingScene>()!.Enabled = false;
            travelState.GetFirstActionOfType<PlayAudioEvent>()!.Enabled = false;
            travelState.GetFirstActionOfType<Wait>()!.time = 1.5f;
            
            FsmState noTravelState = fsm.GetState("Sound Without")!;
            noTravelState.GetFirstActionOfType<PlayAudioEvent>()!.Enabled = false;
            noTravelState.GetFirstActionOfType<Wait>()!.time = 1.5f;
        }

        else if (fsm is { FsmName: "Cutscene Control", name: "_SceneManager" } )
        {
            FsmState waitState = fsm.GetState("Wait")!;

            waitState.GetLastActionOfType<Wait>()!.time = 3f;
            waitState.GetFirstActionOfType<Wait>()!.time = 0.5f;
            waitState.GetFirstActionOfType<ScreenFader>()!.duration = 0.5f;

            FsmState fadeOutState = fsm.GetState("Fade Out")!;
            
            fadeOutState.GetFirstActionOfType<Wait>()!.time = 0.3f;
            fadeOutState.GetFirstActionOfType<ScreenFader>()!.duration = 0.3f;
            fadeOutState.GetFirstActionOfType<PlayAudioEvent>()!.Enabled = false;
            fadeOutState.GetLastActionOfType<Wait>()!.Enabled = false;
        }
    }

    internal static void ShermaAct3Intro(PlayMakerFSM fsm)
    {
        if (!Configs.SkipCutscene.Value)
            return;

        if (fsm is not { FsmName: "Cutscene Control", name: "door_act3_wakeUp" }) 
            return;

        fsm.GetState("Fade Pause")!.DisableActionsOfType<Wait>();
        fsm.GetState("Sherma Start")!.GetFirstActionOfType<Wait>()!.time = 1.5f;
        FsmState fadeInState = fsm.GetState("Fade In")!;
        fadeInState.GetFirstActionOfType<ScreenFader>()!.duration = 2.5f;
        fadeInState.GetFirstActionOfType<CameraBlurPlaneFade>()!.Duration = 2f;
        fadeInState.GetFirstActionOfType<Wait>()!.time = 4f;
        fsm.GetState("Get Up")!.GetFirstActionOfType<Wait>()!.time = 0.5f;
    }

    internal static void DivingBell(PlayMakerFSM fsm)
    {
        if (!Configs.SkipCutscene.Value)
            return;

        if (fsm is { FsmName: "Dialogue", name: "Wake Up Scene", gameObject.scene.name: "Room_Diving_Bell_Abyss"})
        {
            fsm.ChangeTransition("Wait", FsmEvent.Finished.Name, "Fade Up");
            fsm.GetState("Wait")!.GetFirstActionOfType<Wait>()!.time = 1f;

            FsmState fadeUpState = fsm.GetState("Fade Up")!;
            fadeUpState.GetFirstActionOfType<ScreenFaderAlpha>()!.Duration = 1f;
            fadeUpState.GetFirstActionOfType<Wait>()!.time = 1.5f;

            fsm.ChangeTransition("Fade Up", FsmEvent.Finished.Name, "Get Up");
            fsm.GetState("Get Up")!.AddMethod((action) =>
            {
                HeroController.instance.RegainControl();
                if (!HudCanvas.IsVisible) {
                    // Why TC
                    HudCanvas canvas = HudCanvas.instance;
                    canvas.targetFsm.SendEvent("IN");
                    FSMUtility.SendEventToGameObject(canvas.gameObject, "INVENTORY OPEN COMPLETE", true);
                }
            });
        }

        if (fsm is { FsmName: "Sequence", name: "Travel Sequence Control" })
        {
            // Departure
            fsm.GetState("Depressed")!.GetFirstActionOfType<Wait>()!.time = 0.5f;
            fsm.GetState("Hornet Grab")!.DisableActionsOfType<Wait>();
            fsm.GetState("Descend Sequence Fixed")!.DisableActionsOfType<Wait>();

            FsmState fadeState = fsm.GetState("Fade To Black")!;
            fadeState.DisableActionsOfType<Wait>();
            Wait wait = fadeState.GetLastActionOfType<Wait>()!;
            wait.time = 1.5f;
            wait.Enabled = true;

            FsmState cinematicState = fsm.GetState("Cut To Cinematic")!;
            cinematicState.GetFirstActionOfType<ScreenFader>()!.duration =
            cinematicState.GetFirstActionOfType<Wait>()!.time = 1f;
            cinematicState.GetLastActionOfType<Wait>()!.time = 1.5f;

            fsm.GetState("Fade To End")!.GetFirstActionOfType<Wait>()!.time = 1.5f;

            // Arrival
            FsmState fadeUpState = fsm.GetState("Fade Up")!;
            fadeUpState.DisableActionsOfType<Wait>();
            Wait fadeWait = fadeUpState.GetLastActionOfType<Wait>()!;
            fadeWait.time = 0.5f;
            fadeWait.Enabled = true;
        }
    }

    internal static void CitadelEntrance(PlayMakerFSM fsm)
    {
        if (!Configs.SkipCutscene.Value || fsm is not { FsmName: "Cutscene Control", name: "right_cutsceneEntry" })
            return;

        fsm.ChangeTransition("State 1", "FINISH ENTRY", "Fade In");
        fsm.GetFirstActionOfType<ScreenFader>("Fade In")!.duration = 0.5f;
        fsm.GetFirstActionOfType<Wait>("Fade In")!.time = 0.5f;
        fsm.ChangeTransition("Fade In", FsmEvent.Finished.Name, "Give Control");

        fsm.DisableActions("State 1", 2, 3, 4);
    }

}
