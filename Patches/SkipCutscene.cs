using System.Linq;
using HarmonyLib;

namespace QoL.Patches
{
    [HarmonyPatch(typeof(InputHandler), nameof(InputHandler.SetSkipMode))]
    internal class InputHandlerPatch
    {
        private static readonly string[] UnskipScene = { "Bone_East_Umbrella", "Belltown", "Room_Pinstress", "Belltown_Room_pinsmith", "Belltown_Room_doctor", "End_Credits_Scroll", "End_Credits", "Menu_Credits", "End_Game_Completion", "PermaDeath", "Bellway_City", "City_Lace_cutscene" };

        [HarmonyPrefix]
        static bool Prefix(InputHandler __instance, ref GlobalEnums.SkipPromptMode newMode)
        {
            if (!QoLPlugin.SkipCutscene.Value || UnskipScene.Contains(GameManager.instance.sceneName)) return true;
            
            __instance.SkipMode = newMode = GlobalEnums.SkipPromptMode.SKIP_INSTANT;
            __instance.skipCooldownTime = 0f;
            __instance.readyToSkipCutscene = true;
            return false;
        }
    }

    [HarmonyPatch(typeof(SkippableSequence))]
    class SkippableSequencePatch
    {
        [HarmonyPatch(typeof(SkippableSequence), nameof(SkippableSequence.CanSkip), MethodType.Getter)]
        [HarmonyPrefix]
        static bool Prefix(SkippableSequence __instance, ref bool __result)
        {
            if (!QoLPlugin.SkipCutscene.Value) return true;
            __instance.AllowSkip();
            __instance.canSkip = __result = true;
            return false;
        }
    }
}