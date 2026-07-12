namespace QoL.FSMEdits;

internal static class FasterNpc
{
    internal static void FasterNPC(PlayMakerFSM fsm)
    {
        var statueType = Configs.FasterCogworkStatues.Value;
        if (statueType != Configs.FastCogworkStatues.None && fsm is { FsmName: "Sequence", name: "Boss Scene", gameObject.scene.name: "Cog_Dancers"})
        {
            if (!PlayerData.instance.defeatedCogworkDancers && statueType != Configs.FastCogworkStatues.LiftUnlocked)
                fsm.ChangeTransition("Repeat?", "CANCEL", "All Talk Choose Dlg");
            
            if (statueType != Configs.FastCogworkStatues.QuestUnlocked)
                fsm.ChangeTransition("Statues End Singing", FsmEvent.Finished.Name, "Return Control");
        }
        
        else if (Configs.FasterLaceAct2.Value && fsm is { FsmName: "Control", name: "Lace NPC Citadel Meet" })
        {
            bool MistEntrance = fsm.gameObject.scene.name == "Song_20";
            fsm.DisableActionsOfType<SetMeshRenderer>("Init");
            fsm.AddMethod("Init", (action) =>
            {
                fsm.transform.position += MistEntrance ? new(5.86f, 0f, 0f) : new(-6f, 0f, 0f); // Lace pos
                if (MistEntrance) fsm.transform.GetChild(0).localPosition += new Vector3(-2f, 0f); // Cutscene trigger
            });

            fsm.DisableActionsOfType<HeroPlayLookUpAnim>("Look Up");
            fsm.DisableActionsOfType<Wait>("Look Up");
            fsm.ChangeTransition("Look Up", FsmEvent.Finished.Name, "Wait");
            fsm.DisableActionsOfType<Wait>("Wait");
            fsm.ChangeTransition("Wait", FsmEvent.Finished.Name, "Jump Antic");
            fsm.AddAction("Jump Antic", new Wait()
            {
                time = 0.1f,
                finishEvent = FsmEvent.Finished
            });

            fsm.DisableActionsOfType<Wait>("Wait");

            fsm.DisableActionsOfType<Wait>("Quest Update");
            fsm.GetFirstActionOfType<Wait>("Wake Rumble")!.time = 0.5f;
        }

        else if (Configs.FasterMelodyObtain.Value && fsm is { FsmName: "Cyllinder States", name: "puzzle cyllinders" })
        {
            fsm.DisableActionsOfType<Wait>("Singing 2");
            fsm.GetFirstActionOfType<Wait>("BG Choir On")!.enabled = false;
            fsm.GetLastActionOfType<Wait>("BG Choir On")!.time = 2f;
        }
        
        if (fsm.FsmName != "Dialogue")
            return;

        if (Configs.FasterSeamstress.Value && fsm.gameObject is { name: "Seamstress", scene.name: "Bone_East_Umbrella" })
        {
            Plugin.Logger.LogDebug("Modifying Seamstress Dialogue");
            fsm.GetState("DLG After Dress")!.DisableAction(0);
            fsm.gameObject.transform.Find("Exit Lore Wall").localPosition = new Vector3(-30f, 1.91f, 0f);
        }
        
        else if (Configs.FasterCaretaker.Value && fsm.gameObject is { name: "Enclave Caretaker FirstMeet", scene.name: "Song_Enclave" })
        {
            Plugin.Logger.LogDebug("Modifying Caretaker Dialogue");
            if (PlayerData.instance.bellShrineEnclave)
                PlayerData.instance.metCaretaker = true;
        }
        
        else if (Configs.FasterShakra.Value && fsm.gameObject.name == "Mapper NPC")
        {
            Plugin.Logger.LogDebug("Modifying Shakra Dialogue");
            PlayerData.instance.metMapper = true;
        }

        else if (fsm.gameObject.name == "Librarian")
        {
            if (fsm.GetState("Needolin Pre Wait") == null)
                return;

            var board = fsm.GetComponent<RelicBoardOwner>();
            if (Configs.FasterVaultkeeper.Value && (board.IsAnyRelicsToDeposit() || board.list.SelectMany(type => type.Relics).Any(relic => relic.SavedData.IsDeposited)))
            {
                fsm.ChangeTransition("Init", FsmEvent.Finished.Name, "Roar End Small");
                fsm.DisableActionsOfType<CheckTrackTriggerCount>("Idle");
            }

            if (Configs.FasterMelodyObtain.Value)
            {
                fsm.DisableActionsOfType<Wait>("Needolin Pre Wait");
                fsm.ChangeTransition("Needolin Pre Wait", FsmEvent.Finished.Name, "Needolin");
            }
        }

        else if (Configs.FasterMelodyObtain.Value && fsm.gameObject.name == "Last Conductor NPC")
        {
            fsm.GetLastActionOfType<Wait>("Run Melody Play Prompted")!.time = 4f;
            fsm.ChangeTransition("Run Melody Play Prompted", FsmEvent.Finished.Name, "Set Learned Melody 2");
            fsm.InsertMethod("Playing Wait", 1, _ =>
            {
                EventRegister.SendEvent("NEEDOLIN LOCK");
                fsm.SendEvent("MELODY");
            });
            fsm.AddTransition("Playing Wait", "MELODY", "Run Melody Play Prompted");
            fsm.InsertMethod("End Dialogue", 1, _ =>
            {
                EventRegister.SendEvent("FSM CANCEL");
                HeroController.instance.RegainControl();
                if (!HudCanvas.IsVisible) {
                // Why TC
                HudCanvas canvas = HudCanvas.instance;
                canvas.targetFsm.SendEvent("IN");
                FSMUtility.SendEventToGameObject(canvas.gameObject, "INVENTORY OPEN COMPLETE", true);
            }
            });
        }
    }
}
