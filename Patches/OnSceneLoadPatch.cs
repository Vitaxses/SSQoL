using System.Collections;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace QoL.Patches
{
    // SceneLoad (Weakness Scene, Camera pan issue, OldPatch & Fast Shakra)
    public class OnSceneLoadPatch
    {
        public static void OnSceneLoad(Scene scene, LoadSceneMode lsm)
        {
            if (GameManager.instance == null) return;
            if (scene.name.Equals("Bone_04")) PlayerData.instance.metMapper = true;
            GameManager.instance.StartCoroutine(Delay(() =>
            {
                var LName = GameManager.instance.sceneName.ToLower();
                OldPatch(LName);
                SmallTweaks(LName);
                SkipWeakness(LName);
            }, 0.4f));
        }

        private static void SmallTweaks(string LName)
        {
            if (!QoLPlugin.SmallTweaks.Value) return;
            if (LName.Equals("aqueduct_01"))
            {
                GameObject.Destroy(GameObject.Find("Camera Locks"));
            }
        }

        private static void OldPatch(string LName)
        {
            if (!QoLPlugin.OldPatch.Value) return;
            if (LName.Equals("under_17"))
            {
                GameObject obj = GameObject.Find("terrain collider (15)");
                obj.transform.position = obj.transform.localPosition = new Vector3(12.25f, 7.64f, 0f);
                GameObject.Destroy(obj.GetComponent<NonSlider>());
            }
        }

        private static void SkipWeakness(string LName)
        {
            if (!QoLPlugin.SkipWeakness.Value) return;
            GameObject WeaknessManager = GameObject.Find("Weakness Scene");
            switch (LName)
            {
                case "bonetown":
                    Fsm fsm = FSMUtility.GetFSM(GameObject.Find("Churchkeeper Intro Scene")).Fsm;
                    fsm.SetState("Set End");
                    break;
                case "cog_09_destroyed":
                    WeaknessManager = GameObject.Find("Weakness Cog Drop Scene");
                    break;
            }
            if (WeaknessManager != null) WeaknessManager.SetActive(false);
        }

        private static IEnumerator Delay(System.Action action, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            action.Invoke();
            yield break;
        }

    }
}