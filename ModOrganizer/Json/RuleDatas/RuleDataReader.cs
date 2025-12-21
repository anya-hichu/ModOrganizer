using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.RuleDatas;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.RuleExports;

public class RuleDataReader(IPluginLog pluginLog) : Reader<RuleData>(pluginLog)
{
    public override bool TryRead(JsonElement jsonElement, [NotNullWhen(true)] out RuleData? instance)
    {
        instance = null;

        if (!Assert.IsValue(jsonElement, JsonValueKind.Object)) return false;
        if (!Assert.IsValuePresent(jsonElement, nameof(RuleData.Path), out var path)) return false;

        bool? enabled = jsonElement.TryGetProperty(nameof(RuleData.Enabled), out var enabledProperty) ? enabledProperty.GetBoolean() : null;

        int? priority = jsonElement.TryGetProperty(nameof(RuleData.Priority), out var priorityProperty) ? priorityProperty.GetInt32() : null;
        var matchExpression = jsonElement.TryGetProperty(nameof(RuleData.MatchExpression), out var matchExpressionProperty) ? matchExpressionProperty.GetString() : null;
        var pathTemplate = jsonElement.TryGetProperty(nameof(RuleData.PathTemplate), out var pathTemplateProperty) ? pathTemplateProperty.GetString() : null;

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
