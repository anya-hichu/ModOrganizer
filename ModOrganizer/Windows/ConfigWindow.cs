using System;
using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;
using System.Linq;

namespace ModOrganizer.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Config Config { get; init; }

    public ConfigWindow(Config config) : base("ModOrganizer - Config##configWindow")
    {
        SizeConstraints = new()
        {
            MinimumSize = new(375, 330),
            MaximumSize = new(float.MaxValue, float.MaxValue)
        };

        Config = config;
    }

    public void Dispose() { }

    public override void Draw()
    {
        var autoProcessEnabled = Config.AutoProcessEnabled;
        if (ImGui.Checkbox("Enable auto-process##autoProcessEnabled", ref autoProcessEnabled))
        {
            Config.AutoProcessEnabled = autoProcessEnabled;
            Config.Save();
        }

        var autoProcessDelayMs = Config.AutoProcessDelayMs;
        if (ImGui.InputUInt("Delay auto-process (ms)##autoProcessDelayMs", ref autoProcessDelayMs, 50))
        {
            Config.AutoProcessDelayMs = autoProcessDelayMs;
            Config.Save();
        }

        if (ImGui.Button("New rule##newRule"))
        {
            Config.Rules.Add(new());
            Config.Save();
        }

        foreach (var rule in Config.Rules.OrderByDescending(r => r.Priority))
        {
            var hash = rule.GetHashCode();

            var name = rule.Name;
            if (ImGui.InputText($"Name##rule{hash}name", ref name))
            {
                rule.Name = name;
                Config.Save();
            }

            var enabled = rule.Enabled;
            if (ImGui.Checkbox($"Enable##rule{hash}enabled", ref enabled))
            {
                rule.Enabled = enabled;
                Config.Save();
            }

            var priority = rule.Priority;
            if (ImGui.InputInt($"Priority##rule{hash}priority", ref priority, 1, 1))
            {
                rule.Priority = priority;
                Config.Save();
            }

            var matchExpression = rule.MatchExpression;
            if (ImGui.InputTextMultiline($"Match expression##rule{hash}matchExpression", ref matchExpression))
            {
                rule.MatchExpression = matchExpression;
                Config.Save();
            }

            var pathTemplate = rule.PathTemplate;
            if (ImGui.InputTextMultiline($"Path template##rule{hash}pathTemplate", ref pathTemplate))
            {
                rule.PathTemplate = pathTemplate;
                Config.Save();
            }
        }
    }
}
