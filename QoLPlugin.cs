using BepInEx;
using BepInEx.Logging;
using FsmUtilPlugin = Silksong.FsmUtil.Plugin;
using Silksong.ModMenu;
using Silksong.ModMenu.Elements;
using Silksong.ModMenu.Models;

namespace QoL;

[BepInDependency(FsmUtilPlugin.Id)]
[BepInDependency(ModMenuPlugin.Id)]
[BepInAutoPlugin(id: "io.github.vitaxses.qol")]
[ModMenuIgnore]
public sealed partial class QoLPlugin : BaseUnityPlugin
{
    private readonly Harmony harmony = new(Id);

	internal static new ManualLogSource Logger { get; private set; } = null!;

    private void Awake()
    {
		Logger = base.Logger;

        Configs.Bind(Config);
        harmony.PatchAll();
        
        Silksong.ModMenu.Registry.AddModMenu("QoL", Menu);

        Logger.LogInfo($"Plugin {Name} ({Id}) v{Version} has loaded!");
    }

    private SelectableElement Menu() => new TextButton(QoLMenu.Create(Config));
}
