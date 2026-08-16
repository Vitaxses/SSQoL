namespace QoL.FSMEdits;

public static class FastMenu
{
    internal static void ShopUI(PlayMakerFSM fsm)
    {
        if (!Configs.FastUI.Value)
            return;

        FsmFloat fadeTime = fsm.FindFloatVariable("Fade Time")!;
        fadeTime?.RawValue = 0.1f;

        if (fsm.FsmName == "ui_list_item" && (fsm.name == "No" || fsm.name == "Yes"))
        {
            fsm.GetState("Chosen")!.GetLastActionOfType<Wait>()!.time = 0f;
        }

        else if (fsm.FsmName == "Confirm Control" && fsm.name == "UI List")
        {
            fsm.GetState("Particles")!.GetFirstActionOfType<Wait>()!.time = 0.1f;
        }
    }

    internal static void GetMapPrompt(PlayMakerFSM fsm)
    {
        if (!Configs.FastUI.Value)
            return;

        if (fsm is not { name: "UI Msg Get Map(Clone)", FsmName: "Msg Control"})
            return;

        fsm.ChangeTransition("Init", FsmEvent.Finished.Name, "Done");
    }

    internal static void QuestUIPrompt(PlayMakerFSM fsm)
    {
        if (!Configs.FastUI.Value)
            return;

        if (fsm.FsmName != "Control" || (fsm.name != "Wish Granted Prompt New(Clone)" && fsm.name != "Wish Promised Prompt(Clone)"))
        {
            return;
        }

        fsm.gameObject.transform.GetChild(1).GetComponent<Animator>().speed = 2f;

        fsm.GetState("Idle")!.GetFirstActionOfType<Wait>()!.time = 1f;
        fsm.GetState("Fade Down")!.DisableActionsOfType<Wait>();
        fsm.GetState("Explainer Up")!.DisableActionsOfType<Wait>();

        if (fsm.name == "Wish Granted Prompt New(Clone)") // Fix glow issue
            fsm.GetState("Press")!.GetFirstActionOfType<ActivateGameObject>()!.Enabled = false;
    }

}
