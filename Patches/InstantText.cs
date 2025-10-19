using HarmonyLib;

namespace QoL.Patches
{
    [HarmonyPatch(typeof(DialogueBox), nameof(DialogueBox.Start))]
    internal class DialogueBoxPatch
    {
        [HarmonyPostfix]
        static void Postfix(DialogueBox __instance)
        {
            if (QoLPlugin.InstantText.Value)
            {
                __instance.currentRevealSpeed = __instance.regularRevealSpeed = __instance.fastRevealSpeed *= 50;
                __instance.animator.speed = 10f;
            }
        }
    }
}