using HarmonyLib;
namespace QoL.Patches
{
    [HarmonyPatch(typeof(LiftControl), nameof(LiftControl.Start))]
    internal class LiftControlPatch
    {

        [HarmonyPostfix]
        static void Postfix_Start(LiftControl __instance)
        {
            if (!QoLPlugin.FasterLifts.Value || GameManager.instance.sceneName.Equals("Ward_01")) return;

            if (GameManager.instance.sceneName.Equals("Library_11")) __instance.moveSpeed = 25f;
            else __instance.moveSpeed = 150f;
            __instance.moveDelay = 0f;
            __instance.endDelay = 0f;
        }
    }
}