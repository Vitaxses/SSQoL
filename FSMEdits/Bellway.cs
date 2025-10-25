using GlobalEnums;

using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace QoL.FSMEdits;

internal static class Bellway
{
    private static bool IsInBellwayScene(Component component) =>
        FastTravelScenes._scenes.ContainsValue(
            GameManager.InternalBaseSceneName(component.gameObject.scene.name)
        );

    internal static void BellBeast(PlayMakerFSM fsm)
    {
        if (fsm is not { FsmName: "Interaction", name: "Bone Beast NPC" } || !IsInBellwayScene(fsm))
            return;

        Plugin.Logger.LogDebug("Modifying Bell Beast FSM");

        if (Configs.FasterBellwayTravel.Value) {
            // Fast arrival
            // TC made AudioPlayInState (index 6) does block finish so we can't directly disable it here
            fsm.GetAction<Wait>("Travel Arrive Start", 7)!.time = 0f;

            // HUD Fix
            fsm.AddMethod("Wait Finished Entering", (_) => {
                if (!HudCanvas.IsVisible) {
                    // Why TC
                    HudCanvas canvas = HudCanvas.instance;
                    canvas.targetFsm.SendEvent("IN");
                    FSMUtility.SendEventToGameObject(canvas.gameObject, "INVENTORY OPEN COMPLETE", true);
                }
            });

            // Fast departure
            fsm.GetAction<ScreenFader>("Hero Jump", 0)!.duration = 0.25f;
            fsm.DisableAction("Hero Jump", 5);
            fsm.ChangeTransition("Hero Jump", FsmEvent.Finished.Name, "Time Passes");

            // Don't sing on first entrance in range
            fsm.ChangeTransition("First Enter?", FsmEvent.Finished.Name, "Idle");
        }

        if (Configs.BellBeastFreeWill.Value)
        {
            // Standby everywhere
            EnumCompare actionCompareLocation = fsm.GetAction<EnumCompare>("Is Already Present?", 1)!;
            actionCompareLocation.notEqualEvent = actionCompareLocation.equalEvent;

            // Update current location when entering range
            fsm.InsertMethod("First Enter?", 0, (_) => 
                PlayerData.instance.FastTravelNPCLocation = (FastTravelLocations) actionCompareLocation.compareTo.Value
            );
        }

        if (Configs.NoBellBeastSleep.Value)
        {
            fsm.ChangeTransition("Start State", "SLEEP", "Wake Up");
        }

        // This is effectively covered by SkipCutscene already
        // But here we make the cutscene not loaded at all, which is even faster
        if (Configs.SkipCutscene.Value)
        {
            fsm.DisableAction("Choose Scene", 3); // Don't preload
            fsm.ReplaceAction("Go To Stag Cutscene", 7, new BeginSceneTransition() {
                sceneName = fsm.FsmVariables.GetFsmString("To Scene"),
                entryGateName = "door_fastTravelExit",
                entryDelay = 0f,
                visualization = GameManager.SceneLoadVisualizations.Default,
                preventCameraFadeOut = true
            });
        }

        if (Configs.FasterBellwayBuy.Value)
        {
            // Auto call after unlock
            fsm.ChangeTransition("Can Appear 2", "TRUE", "Appear Delay");

            // Fast call
            fsm.DisableAction("Appear Delay", 5);
            fsm.DisableAction("Start Shake", 8);
        }
    }

    internal static void Toll(PlayMakerFSM fsm)
    {
        if (!Configs.FasterBellwayBuy.Value)
            return;

        // Grand Bellway has "Bellway Toll Machine(1)"
        if (fsm is not { FsmName: "Unlock Behaviour", name: "Bellway Toll Machine" } || !IsInBellwayScene(fsm))
            return;

        Plugin.Logger.LogDebug("Modifying Bellway Toll FSM");

        // Fast strum
        fsm.DisableAction("Return Control", 5);
        fsm.AddAction("Return Control", new SetAnimator() {
            target = new(),
            active = true
        });
        fsm.AddMethod("Return Control", (_) => 
            fsm.GetComponent<Animator>().speed = 20f
        );
        fsm.DisableAction("Sequence Strum", 0);
        fsm.DisableAction("Stop", 1);

        // Fast floor open
        fsm.DisableActions("Open Floor", 3, 5);
        fsm.GetAction<CallMethodProper>("Open Floor", 0)!
            .gameObject.GameObject.Value
            .GetComponent<Animator>().speed = 10f;
    }
}
