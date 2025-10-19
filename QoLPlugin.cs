using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using QoL.Patches;
using UnityEngine.SceneManagement;

namespace QoL;

[BepInAutoPlugin(id: "io.github.vitaxses.qol")]
[BepInDependency("org.silksong-modding.fsmutil", BepInDependency.DependencyFlags.HardDependency)]
public partial class QoLPlugin : BaseUnityPlugin
{
    Harmony harmony;
    public static QoLPlugin Instance;


    public static ConfigEntry<bool> FasterBellwayBuy { get; private set; }
    public static ConfigEntry<bool> NoBellBeastSleep { get; private set; }
    public static ConfigEntry<bool> BellBeastFreeWill { get; private set; }

    public static ConfigEntry<bool> FasterNPC { get; private set; }
    public static ConfigEntry<bool> FasterBossLoad { get; private set; }


    public static ConfigEntry<bool> InstantLevers { get; private set; }
    public static ConfigEntry<bool> FasterLifts { get; private set; }
    public static ConfigEntry<bool> InstantText { get; private set; }
    public static ConfigEntry<bool> SkipCutscene { get; private set; }
    public static ConfigEntry<bool> SkipWeakness { get; private set; }
    public static ConfigEntry<bool> SmallTweaks { get; private set; }
    public static ConfigEntry<bool> OldPatch { get; private set; }
    public static ConfigEntry<bool> FastUI { get; private set; }
    public static ConfigEntry<bool> NoHardFalls { get; private set; }

    public ManualLogSource Logger => base.Logger;

    private void Start()
    {
        Instance = this;

        FasterBellwayBuy = Config.Bind("Bellway Settings", "Faster Bellway Buy", true, "Removes The Melody When Buying A Bellway Station");
        NoBellBeastSleep = Config.Bind("Bellway Settings", "No BellBeast Sleep", true, "Removes The Chance Of The BellBeast Sleeping");
        BellBeastFreeWill = Config.Bind("Bellway Settings", "BellBeast Has Free Will", false, "BellBeast Will Always Be At Your Location");

        FasterNPC = Config.Bind("NPC Settings", "Faster Npc", true, "Removes Some Dialogue For Introduction Of An NPC");
        FasterBossLoad = Config.Bind("NPC Settings", "(BETA) Faster Boss Start", false, "(BETA) Remove's Dialogue From Bosses");

        InstantLevers = Config.Bind("Global Settings", "Instant Levers", true, "Removes The Delay When Hitting A Lever");
        FasterLifts = Config.Bind("Global Settings", "Faster Lifts", true, "Lifts Now Have Super Speed");
        InstantText = Config.Bind("Global Settings", "Instant Text", true, "Makes the Scroll Speed Of Text and Popup Speed Instant");
        SkipCutscene = Config.Bind("Global Settings", "Skip Cutscenes Faster", true, "Skips Cutscenes Faster");
        SkipWeakness = Config.Bind("Global Settings", "Skip Weakness", true, "Removes Weakness scenes in Moss Grotto And Cogwork Core");
        SmallTweaks = Config.Bind("Global Settings", "Small Tweaks", true, "Fixes Camera Issue In Putrefied Ducts");
        OldPatch = Config.Bind("Global Settings", "Old patch", false, "Patches In Old Features/Skips");
        FastUI = Config.Bind("Global Settings", "Fast Menu", true, "Removes The Fade Delay");
        NoHardFalls = Config.Bind("Global Settings", "No Hard Falls", false, "No More Broken Angles");

        SceneManager.sceneLoaded += OnSceneLoadPatch.OnSceneLoad;

        harmony = new(Id);
        harmony.PatchAll();

        Logger.LogInfo($"Plugin {Name} ({Id}) has loaded!");
    }
}
