using System;
using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;
using System.Linq;
using Dalamud.Interface;
using Dalamud.Plugin;
using ModOrganizer.Shared;

namespace ModOrganizer.Windows;

public class ConfigWindow : Window, IDisposable
{
    private ActionDebouncer ActionDebouncer { get; init; }
    private Config Config { get; init; }
    private IDalamudPluginInterface PluginInterface { get; init; }

    public ConfigWindow(ActionDebouncer actionDebouncer, Config config, IDalamudPluginInterface pluginInterface, Action toggleMainWindow) : base("ModOrganizer - Config##configWindow")
    {
        SizeConstraints = new()
        {
            MinimumSize = new(375, 330),
            MaximumSize = new(float.MaxValue, float.MaxValue)
        };

        TitleBarButtons = [new() 
        { 
            Icon = FontAwesomeIcon.ListAlt, 
            ShowTooltip = () => ImGui.SetTooltip("Toggle main window"), 
            Click = _ => toggleMainWindow() 
        }];

        ActionDebouncer = actionDebouncer;
        Config = config;
        PluginInterface = pluginInterface;
    }

    public void Dispose() { }

    public override void Draw()
    {
        var autoProcessEnabled = Config.AutoProcessEnabled;
        if (ImGui.Checkbox("Enable auto-process##autoProcessEnabled", ref autoProcessEnabled))
        {
            Config.AutoProcessEnabled = autoProcessEnabled;
            SaveConfig();
        }

        var autoProcessWaitMs = Config.AutoProcessWaitMs;
        if (ImGui.InputUInt("Wait auto-process (ms)##autoProcessWaitMs", ref autoProcessWaitMs, 50))
        {
            Config.AutoProcessWaitMs = autoProcessWaitMs;
            SaveConfigLater();
        }

        if (ImGui.Button("New rule##newRule"))
        {
            Config.Rules.Add(new());
            SaveConfig();
        }

        foreach (var rule in Config.Rules.OrderByDescending(r => r.Priority))
        {
            var hash = rule.GetHashCode();

            var enabled = rule.Enabled;
            if (ImGui.Checkbox($"Enable##rule{hash}Enabled", ref enabled))
            {
                rule.Enabled = enabled;
                SaveConfig();
            }

            var name = rule.Name;
            if (ImGui.InputText($"Name##rule{hash}Name", ref name))
            {
                rule.Name = name;
                SaveConfigLater();
            }

            var priority = rule.Priority;
            if (ImGui.InputInt($"Priority##rule{hash}Priority", ref priority, 1, 1))
            {
                rule.Priority = priority;
                SaveConfig();
            }

            var matchExpression = rule.MatchExpression;
            if (ImGui.InputTextMultiline($"Match expression##rule{hash}MatchExpression", ref matchExpression, ushort.MaxValue))
            {
                rule.MatchExpression = matchExpression;
                SaveConfigLater();
            }

            var pathTemplate = rule.PathTemplate;
            if (ImGui.InputTextMultiline($"Path template##rule{hash}PathTemplate", ref pathTemplate, ushort.MaxValue))
            {
                rule.PathTemplate = pathTemplate;
                SaveConfigLater();
            }
        }
    }

    private void SaveConfigLater() => ActionDebouncer.Invoke(nameof(SaveConfig), SaveConfig, TimeSpan.FromSeconds(1));

    private void SaveConfig() => PluginInterface.SavePluginConfig(Config);
}
