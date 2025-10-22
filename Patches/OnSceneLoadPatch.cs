using UnityEngine.SceneManagement;

namespace QoL.Patches;

// SceneLoad (Weakness Scene, Camera pan issue, OldPatch & Fast Shakra)
internal static class OnSceneLoadPatch
{
    internal static void OnSceneLoad(Scene scene, LoadSceneMode lsm)
    {
        if (HeroController.UnsafeInstance == null)
            return;

        if (Configs.FasterNPC.Value && scene.name == "Bone_04")
            PlayerData.instance.metMapper = true;

        SkipWeakness(scene.name);

        StartCoroutine(() =>
        {
            string sceneName = GameManager.instance.sceneName;
            OldPatch(sceneName);
            SmallTweaks(sceneName);
        }, 0.3f);
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

        if (sceneName == "Under_17")
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

        if (sceneName == "Bonetown" && !PlayerData.instance.churchKeeperIntro)
        {
            PlayerData.instance.churchKeeperIntro = true;

            StartCoroutine(() =>
            {
                GameObject.Find("Churchkeeper Intro Scene")
                    .LocateMyFSM("Control")
                    .SetState("Set End");
            }, 0.3f);
        }


        StartCoroutine(() =>
        {
            GameObject weaknessScene = GameObject.Find("Weakness Scene");

            if (sceneName == "Cog_09_Destroyed")
                weaknessScene = GameObject.Find("Weakness Cog Drop Scene");

            if (weaknessScene != null)
                weaknessScene.SetActive(false);
        }, 0.3f);
    }

    private static IEnumerator Delay(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action.Invoke();
    }

    private static void StartCoroutine(Action action, float seconds)
    {
        if (HeroController.UnsafeInstance == null)
            return;
        
        HeroController.instance.StartCoroutine(Delay(seconds, action));
    }
}
