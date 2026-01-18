using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;

using ModOrganizer.Json.RuleDatas;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.RuleExports;

public class RuleDataReader(IPluginLog pluginLog) : Reader<RuleData>(pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out RuleData? instance)
    {
        instance = null;

        if (!IsValue(element, JsonValueKind.Object)) return false;
        if (!IsValuePresent(element, nameof(RuleData.Path), out var path)) return false;

        bool? enabled = element.TryGetProperty(nameof(RuleData.Enabled), out var enabledProperty) ? enabledProperty.GetBoolean() : null;

        int? priority = element.TryGetProperty(nameof(RuleData.Priority), out var priorityProperty) ? priorityProperty.GetInt32() : null;
        var matchExpression = element.TryGetProperty(nameof(RuleData.MatchExpression), out var matchExpressionProperty) ? matchExpressionProperty.GetString() : null;
        var pathTemplate = element.TryGetProperty(nameof(RuleData.PathTemplate), out var pathTemplateProperty) ? pathTemplateProperty.GetString() : null;

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
