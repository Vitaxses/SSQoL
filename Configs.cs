using BepInEx.Configuration;

namespace QoL;

public static class Configs
{
    public static ConfigEntry<bool> FasterBellwayBuy { get; private set; } = null!;
    public static ConfigEntry<bool> NoBellBeastSleep { get; private set; } = null!;
    public static ConfigEntry<bool> BellBeastFreeWill { get; private set; } = null!;

    public static ConfigEntry<bool> FasterNPC { get; private set; } = null!;
    public static ConfigEntry<bool> FasterBossLoad { get; private set; } = null!;

    public static ConfigEntry<bool> InstantLevers { get; private set; } = null!;
    public static ConfigEntry<bool> FasterLifts { get; private set; } = null!;
    public static ConfigEntry<bool> InstantText { get; private set; } = null!;
    public static ConfigEntry<bool> SkipCutscene { get; private set; } = null!;
    public static ConfigEntry<bool> SkipWeakness { get; private set; } = null!;
    public static ConfigEntry<bool> SmallTweaks { get; private set; } = null!;
    public static ConfigEntry<bool> OldPatch { get; private set; } = null!;
    public static ConfigEntry<bool> FastUI { get; private set; } = null!;
    public static ConfigEntry<bool> NoHardFalls { get; private set; } = null!;

    internal static void Bind(ConfigFile config)
    {
        FasterBellwayBuy = config.Bind("Bellway Settings", "Faster Bellway Purchase", true, "Removes The Melody When Buying A Bellway Station");
        NoBellBeastSleep = config.Bind("Bellway Settings", "BellBeast Always Awake", true, "Removes The Chance Of The BellBeast Sleeping");
        BellBeastFreeWill = config.Bind("Bellway Settings", "BellBeast Has Free Will", false, "BellBeast Will Always Be At Your Location");

        FasterNPC = config.Bind("NPC Settings", "Faster Npc", true, "Removes Some Dialogue For Introduction Of An NPC");
        FasterBossLoad = config.Bind("NPC Settings", "(BETA) Faster Boss Start", false, "(BETA) Remove's Dialogue From Bosses");

        InstantLevers = config.Bind("Global Settings", "Instant Levers", true, "Removes The Delay When Hitting A Lever");
        FasterLifts = config.Bind("Global Settings", "Faster Lifts", true, "Lifts Now Have Super Speed");
        InstantText = config.Bind("Global Settings", "Instant Text", true, "Makes the Scroll Speed Of Text and Popup Speed Instant");
        SkipCutscene = config.Bind("Global Settings", "Skip Cutscenes Faster", true, "Skips Cutscenes Faster");
        SkipWeakness = config.Bind("Global Settings", "Skip Weakness", true, "Removes Weakness scenes in Moss Grotto And Cogwork Core");
        SmallTweaks = config.Bind("Global Settings", "Small Tweaks", true, "Fixes Camera Issue In Putrefied Ducts");
        OldPatch = config.Bind("Global Settings", "Old patch", false, "Patches In Old Features/Skips");
        FastUI = config.Bind("Global Settings", "Fast Menu", true, "Removes The Fade Delay");
        NoHardFalls = config.Bind("Global Settings", "No Hard Falls", false, "No More Broken Angles");
    }
}
