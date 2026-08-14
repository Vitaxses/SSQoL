using System.Collections.Generic;

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

    [HarmonyWrapSafe, HarmonyPostfix]
    private static void Postfix_LinkDoors(MazeController __instance, IReadOnlyList<TransitionPoint> totalDoors)
    {
        if (!Configs.OldMist.Value) return;

        OldMist.LinkAllRestSceneExits(__instance, totalDoors);
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

    // LinkDoors wires a single exit to the rest scene and stops looking. Every horizontal exit used to
    // work, so point all of them at it. The exit scene link is left alone, that one really is one door.
    internal static void LinkAllRestSceneExits(MazeController controller, IReadOnlyList<TransitionPoint> totalDoors)
    {
        if (controller.isCapScene || totalDoors == null) return;

        PlayerData pd = PlayerData.instance;
        int correctEntered = pd.CorrectMazeDoorsEntered;

        if (correctEntered >= controller.neededCorrectDoors - 1) return;
        if (correctEntered < controller.restScenePoint - 1 || pd.EnteredMazeRestScene) return;

        if (!SceneTeleportMap.GetTeleportMap().TryGetValue(controller.restSceneName, out SceneTeleportMap.SceneInfo restScene)) return;

        foreach (TransitionPoint door in totalDoors)
        {
            if (door == null) continue;

            string doorName = door.name;

            // Relinking the way back would make juggling out of this room impossible
            if (doorName == pd.PreviousMazeTargetDoor) continue;

            string targetDoorMatch = controller.GetDoorDirMatch(doorName);
            if (targetDoorMatch is not ("left" or "right")) continue;

            string gate = restScene.TransitionGates.FirstOrDefault(g => g.StartsWith(targetDoorMatch, StringComparison.Ordinal));
            if (string.IsNullOrEmpty(gate)) continue;

            door.SetTargetScene(controller.restSceneName);
            door.entryPoint = gate;

            if (!controller.correctDoors.Contains(door)) controller.correctDoors.Add(door);
        }
    }
}
