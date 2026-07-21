namespace QoL.FSMEdits;

internal static class Abilities
{
    internal static void NeedleStrike(PlayMakerFSM fsm)
    {
        if (!Configs.BeastBoosts.Value || fsm is not { FsmName: "Nail Arts"})
            return;
        
        Plugin.Logger.LogDebug("Modifying Needle Strike FSM");

        FsmState leapState = fsm.GetState("Warrior2 Leap")!;
        FsmFloat velocityY = fsm.FindFloatVariable("Velocity Y")!;
        FsmBool wasGroundedBool = fsm.GetBoolVariable("QoL Beast Was Grounded");

        leapState.InsertAction(new ConvertBoolToFloat()
        {
            boolVariable = wasGroundedBool,
            floatVariable = velocityY,
            falseValue = velocityY,
            trueValue = leapState.GetFirstActionOfType<ConvertBoolToFloat>()!.trueValue
        }, 4);

        leapState.AddMethod((action) =>
        {
            wasGroundedBool.RawValue = Configs.BeastBoosts.Value ? fsm.FindBoolVariable("Is Grounded")!.RawValue : false;
        });
    }

    internal static void Bind(PlayMakerFSM fsm)
    {
        if (fsm is not { name: "Hero_Hornet(Clone)", FsmName: "Bind" })
            return;

        var callMethodProper = fsm.GetFirstActionOfType<CallMethodProper>("Sprint?");
        var boolTestMulti = fsm.GetFirstActionOfType<BoolTestMulti>("Sprint?");

        fsm.InsertMethod("Sprint?", () =>
        {
            if (Configs.BindDashRefresh.Value)
            {
                callMethodProper?.Enabled = false;
                boolTestMulti?.Enabled = false;
            } else
            {
                callMethodProper?.Enabled = true;
                boolTestMulti?.Enabled = true;
            }
        }, 0);
    }

    internal static void FaydownNeedolinCheck(PlayMakerFSM fsm)
    {
        if (!Configs.RemoveFaydownNeedolinCheck.Value || fsm is not { FsmName: "DJ Get Sequence"})
            return;
        
        Plugin.Logger.LogDebug("Modifying Faydown Get Sequence FSM");

        fsm.ChangeTransition("Has Needolin?", "FALSE", "Dlg End");
    }
}
