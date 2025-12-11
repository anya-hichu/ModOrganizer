using System;
using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;
using System.Linq;
using Dalamud.Interface;
using Dalamud.Plugin;

namespace ModOrganizer.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Config Config { get; init; }
    private IDalamudPluginInterface PluginInterface { get; init; }

    public ConfigWindow(Config config, IDalamudPluginInterface pluginInterface, Action toggleMainWindow) : base("ModOrganizer - Config##configWindow")
    {
        SizeConstraints = new()
        {
            MinimumSize = new(375, 330),
            MaximumSize = new(float.MaxValue, float.MaxValue)
        };

        TitleBarButtons = [new() { Icon = FontAwesomeIcon.ListAlt, ShowTooltip = () => ImGui.SetTooltip("Toggle main window"), Click = _ => toggleMainWindow() }];

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
            Save(Config);
        }

        var autoProcessDelayMs = Config.AutoProcessDelayMs;
        if (ImGui.InputUInt("Delay auto-process (ms)##autoProcessDelayMs", ref autoProcessDelayMs, 50))
        {
            Config.AutoProcessDelayMs = autoProcessDelayMs;
            Save(Config);
        }

        if (ImGui.Button("New rule##newRule"))
        {
            Config.Rules.Add(new());
            Save(Config);
        }

        foreach (var rule in Config.Rules.OrderByDescending(r => r.Priority))
        {
            var hash = rule.GetHashCode();

            var enabled = rule.Enabled;
            if (ImGui.Checkbox($"Enable##rule{hash}Enabled", ref enabled))
            {
                rule.Enabled = enabled;
                Save(Config);
            }

            var name = rule.Name;
            if (ImGui.InputText($"Name##rule{hash}Name", ref name))
            {
                rule.Name = name;
                Save(Config);
            }

            var priority = rule.Priority;
            if (ImGui.InputInt($"Priority##rule{hash}Priority", ref priority, 1, 1))
            {
                rule.Priority = priority;
                Save(Config);
            }

            var matchExpression = rule.MatchExpression;
            if (ImGui.InputTextMultiline($"Match expression##rule{hash}MatchExpression", ref matchExpression, ushort.MaxValue))
            {
                rule.MatchExpression = matchExpression;
                Save(Config);
            }

            var pathTemplate = rule.PathTemplate;
            if (ImGui.InputTextMultiline($"Path template##rule{hash}PathTemplate", ref pathTemplate, ushort.MaxValue))
            {
                rule.PathTemplate = pathTemplate;
                Save(Config);
            }
        }
    }

    private void Save(Config config) => PluginInterface.SavePluginConfig(config);
}
