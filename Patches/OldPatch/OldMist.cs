namespace QoL.Patches.OldPatch;

[HarmonyPatch(typeof(MazeController), nameof(MazeController.SubscribeDoorEntered))]
internal static class MazeDoorEnteredPatch
{
    [HarmonyWrapSafe, HarmonyPrefix]
    private static bool Prefix_SubscribeDoorEntered(MazeController __instance, TransitionPoint door)
    {
        if (!Configs.OldMist.Value) return true;

        door.OnBeforeTransition += () =>
        {
            OldMist.OnBeforeTransition(__instance, door);
        };

        return false;
    }
}

[HarmonyPatch(typeof(MazeController), nameof(MazeController.LinkDoors))]
internal static class MazeDoorLinkPatch
{
    // While DidEnterPreviousMazeDoor is set, LinkDoors makes the door you came from the only correct one,
    // which is what herds you back into the room you just left. Also clears it out of an older save.
    [HarmonyWrapSafe, HarmonyPrefix]
    private static void Prefix_LinkDoors()
    {
        if (!Configs.OldMist.Value) return;

        PlayerData.instance.DidEnterPreviousMazeDoor = false;
    }
}

internal static class OldMist
{
    internal static void OnBeforeTransition(MazeController controller, TransitionPoint door)
    {
        string doorName = door.name;
        PlayerData pd = PlayerData.instance;

        if (!controller.isCapScene)
        {
            bool isBackDoor = pd.PreviousMazeTargetDoor == doorName;

            if (door.targetScene == controller.restSceneName)
            {
                pd.EnteredMazeRestScene = true;
                pd.CorrectMazeDoorsEntered = controller.neededCorrectDoors - controller.restScenePoint;
                pd.IncorrectMazeDoorsEntered = 0;
            }
            else if (controller.correctDoors.Contains(door))
            {
                pd.CorrectMazeDoorsEntered++;
                pd.IncorrectMazeDoorsEntered = 0;
            }
            // A back door that isn't the correct door falls through, so the counters survive the trip
            else if (!isBackDoor)
            {
                pd.CorrectMazeDoorsEntered = 0;
                pd.IncorrectMazeDoorsEntered++;
                pd.EnteredMazeRestScene = false;

                if (pd.IncorrectMazeDoorsEntered >= controller.allowedIncorrectDoors && doorName.StartsWith("right", StringComparison.Ordinal))
                {
                    door.SetTargetScene("Dust_Maze_09_entrance");
                    door.entryPoint = "left1";
                }
            }

            pd.DidEnterPreviousMazeDoor = false;
        }

        pd.PreviousMazeTargetDoor = door.entryPoint;
        pd.PreviousMazeScene = door.gameObject.scene.name;
        pd.PreviousMazeDoor = doorName;
    }
}
