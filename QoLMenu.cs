using System.Collections.Generic;
using BepInEx.Configuration;
using Silksong.ModMenu.Elements;
using Silksong.ModMenu.Models;
using Silksong.ModMenu.Screens;

namespace QoL;

public static class QoLMenu
{   
    public static ScrollingMenuScreen Create(ConfigFile config)
    {
        var screen = new ScrollingMenuScreen("QoL");

        var sectionEntries = config
            .GroupBy(entry => entry.Value.Definition.Section)
            .Where(g => g.Any())
            .ToList();

        foreach (var section in sectionEntries)
        {
            var screenBuilder = new ScrollingMenuScreen(section.Key == "NPC Modules" ? "Boss Modules" : section.Key);
            foreach (var entry in section)
            {
                var settingType = entry.Value.SettingType;

                if (settingType == typeof(bool))
                    screenBuilder.Add(Bool((ConfigEntry<bool>)entry.Value));

                else if (settingType == typeof(Configs.FastCogworkStatues))
                    screenBuilder.Add(Enum<Configs.FastCogworkStatues>(entry.Value));

                else if (settingType == typeof(Configs.LiftSpeed))
                    screenBuilder.Add(Enum<Configs.LiftSpeed>(entry.Value));
            }

            screen.Add(new TextButton(screenBuilder));
        }

        return screen;
    }

    private static ChoiceElement<bool> Bool(ConfigEntry<bool> entry)
    {
        var model = new ListChoiceModel<bool>([true, false])
        {
            DisplayFn = (idx, val) => val ? "Enabled" : "Disabled"
        };

        var element = new ChoiceElement<bool>
        (
            entry.Definition.Key,
            model,
            entry.Description.Description
        );

        model.SetValue(entry.Value);
        model.OnValueChanged += v => entry.Value = v;

        return element;
    }

    private static ChoiceElement<T> Enum<T>(ConfigEntryBase entryBase) where T : Enum
    {
        var values = new List<T>((T[])System.Enum.GetValues(typeof(T)));
        var model = new ListChoiceModel<T>(values);

        var entry = (ConfigEntry<T>)entryBase;

        var element = new ChoiceElement<T>
        (
            entry.Definition.Key,
            model,
            entry.Description.Description
        );

        model.SetValue(entry.Value);
        model.OnValueChanged += v => entry.Value = v;

        return element;
    }
}