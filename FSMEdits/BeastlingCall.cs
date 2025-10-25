using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace QoL.FSMEdits;

internal static class BeastlingCall {
    internal static void SilkSpecials(PlayMakerFSM fsm)
    {
        if (!Configs.FasterBeastlingCall.Value)
            return;

        if (fsm is not { FsmName: "Silk Specials", gameObject: { name: "Hero_Hornet(Clone)", scene.name: nameof(UObject.DontDestroyOnLoad) } })
            return;

        fsm.DisableActions("Hornet Jump Antic", 1, 4, 5);
        fsm.DisableActions("Hornet Jump", 1, 2, 3, 4, 5, 6, 7, 8);
        fsm.AddTransition("Hornet Jump", FsmEvent.Finished.Name, "Hornet Fall");
        fsm.DisableActions("Hornet Fall", 0, 3, 5, 6, 7);
        fsm.AddTransition("Hornet Fall", FsmEvent.Finished.Name, "Children Leave Fade");

        fsm.GetAction<ScreenFader>("Children Leave Fade", 6)!.duration = 0.25f;
        fsm.GetAction<Wait>("Children Leave Fade", 7)!.time = 0.25f;
    }

    internal static void Beastlings(PlayMakerFSM fsm)
    {
        if (!Configs.FasterBeastlingCall.Value)
            return;

        if (fsm is not { FsmName: "bellbeast_child_teleport_arrive", gameObject: { name: "Bone Beast Children Teleport(Clone)", scene.name: nameof(UObject.DontDestroyOnLoad) } })
            return;

        fsm.AddMethod("Init", (_) => {
            // Somehow Play(clip, 0f) does not reset clip time if the clip is
            // current clip, so do reset them here.
            MirrorTk2dAnimDelayed mirrorer = fsm.GetComponent<MirrorTk2dAnimDelayed>();
            mirrorer.mirrorAnimator.PlayFromFrame(0);
            mirrorer.animator.PlayFromFrame(0);
            mirrorer.animator.Stop(); // This will be started by the mirrorer later
        });
    }

    internal static void Needolin(PlayMakerFSM fsmSilkSpecials)
    {
        if (!Configs.FasterBeastlingCall.Value && !Configs.SkipBeastlingCallPerformance.Value)
            return;

        if (fsmSilkSpecials is not { FsmName: "Silk Specials", gameObject: { name: "Hero_Hornet(Clone)", scene.name: nameof(UObject.DontDestroyOnLoad) } })
            return;

        // Needolin SubFSM
        Fsm fsm = fsmSilkSpecials.GetAction<RunFSM>("Needolin Sub", 2)!.runFsm;

        fsm.GetAction<BoolTestDelay>("Needolin FT Wait", 4)!.delay = 0f;
        fsm.GetAction<Wait>("Can Fast Travel?", 1)!.time = 0f;

        fsm.GetAction<Wait>("Needolin FT Antic", 5)!.time = // Originally 3f
            Configs.SkipBeastlingCallPerformance.Value ? 0f : 1.5f;
    }
}
