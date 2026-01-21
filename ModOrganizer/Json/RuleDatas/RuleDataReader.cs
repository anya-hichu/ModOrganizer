using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Elements;
using ModOrganizer.Json.RuleDatas;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.RuleExports;

public class RuleDataReader(IPluginLog pluginLog) : Reader<RuleData>(pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out RuleData? instance)
    {
        instance = null;

        if (!element.Is(JsonValueKind.Object, PluginLog)) return false;

        if (!element.TryGetRequiredPropertyValue(nameof(RuleData.Enabled), out bool enabled, PluginLog)) return false;
        if (!element.TryGetRequiredPropertyValue(nameof(RuleData.Path), out string? path, PluginLog)) return false;
        if (!element.TryGetRequiredPropertyValue(nameof(RuleData.Priority), out int priority, PluginLog)) return false;
        if (!element.TryGetRequiredPropertyValue(nameof(RuleData.MatchExpression), out string? matchExpression, PluginLog)) return false;
        if (!element.TryGetRequiredPropertyValue(nameof(RuleData.PathTemplate), out string? pathTemplate, PluginLog)) return false;

        instance = new()
        {
            Enabled = enabled,
            Path = path,
            Priority = priority,
            MatchExpression = matchExpression,
            PathTemplate = pathTemplate
        };

        return true;
    }
}
