using HutongGames.PlayMaker;

using UnityEngine.SceneManagement;

namespace QoL.Patches;

// SceneLoad (Weakness Scene, Camera pan issue, OldPatch & Fast Shakra)
internal static class OnSceneLoadPatch
{
    internal static void OnSceneLoad(Scene scene, LoadSceneMode lsm)
    {
        if (HeroController.UnsafeInstance == null)
            return;

        if (scene.name == "Bone_04")
            PlayerData.instance.metMapper = true;

        string sceneName = GameManager.instance.sceneName;
        SkipWeakness(sceneName);

        GameManager.instance.StartCoroutine(Delay(0.3f, () =>
        {
            OldPatch(sceneName);
            SmallTweaks(sceneName);
        }));
    }

    private static void SmallTweaks(string sceneName)
    {
        if (!Configs.SmallTweaks.Value || sceneName != "Aqueduct_01")
            return;

        UObject.Destroy(GameObject.Find("Camera Locks"));
    }

    private static void OldPatch(string sceneName)
    {
        if (!Configs.OldPatch.Value)
            return;

        if (sceneName.Equals("Under_17"))
        {
            GameObject obj = GameObject.Find("terrain collider (15)");
            obj.transform.position = new Vector3(12.25f, 7.64f, 0f);
            UObject.Destroy(obj.GetComponent<NonSlider>());
        }
    }

    private static void SkipWeakness(string sceneName)
    {
        if (!Configs.SkipWeakness.Value)
            return;

        PlayerData.instance.churchKeeperIntro = true;
        HeroController.instance.StartCoroutine(Delay(0.4f, () =>
        {
            GameObject weaknessScene = GameObject.Find("Weakness Scene");
            switch (sceneName)
            {
                case "Bonetown":
                    GameObject.Find("Churchkeeper Intro Scene")
                        .LocateMyFSM("Control")
                        .Fsm.SetState("Set End");
                    break;
                case "Cog_09_Destroyed":
                    weaknessScene = GameObject.Find("Weakness Cog Drop Scene");
                    break;
            }

            if (weaknessScene != null)
            {
                weaknessScene.SetActive(false);
            }
        }));
    }

    private static IEnumerator Delay(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action.Invoke();
    }
}
