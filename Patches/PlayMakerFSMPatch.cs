using System;

using HarmonyLib;

using QoL.FSMEdits;

namespace QoL.Patches
{
    [HarmonyPatch(typeof(PlayMakerFSM), nameof(PlayMakerFSM.Start))]
    internal static class PlayMakerFSMPatch
    {
        private static readonly Action<PlayMakerFSM>[] patches =
        [
            FasterBossAndNpc.FasterBoss,
            FasterBossAndNpc.FasterNPC,
            Bellway.BellBeast,
            Bellway.Toll
        ];
        
        [HarmonyPostfix]
        private static void Postfix(PlayMakerFSM __instance) 
        {
            foreach (Action<PlayMakerFSM> patch in patches)
            {
                patch(__instance);
            }
        }
    }
}