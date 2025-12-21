using Dalamud.Plugin.Services;
using ModOrganizer.Json.Writers;
using System.Text.Json;

namespace ModOrganizer.Json.RuleDatas;

public class RuleDataWriter(IPluginLog pluginLog) : Writer<RuleData>(pluginLog)
{
    public override bool TryWrite(Utf8JsonWriter jsonWriter, RuleData instance)
    {
        using var _ = jsonWriter.WriteObject();

        jsonWriter.WriteString(nameof(RuleData.Path), instance.Path);

        if (instance.Enabled.HasValue) jsonWriter.WriteBoolean(nameof(RuleData.Enabled), instance.Enabled.Value);
        if (instance.Priority.HasValue) jsonWriter.WriteNumber(nameof(RuleData.Priority), instance.Priority.Value);
        if (instance.MatchExpression != null) jsonWriter.WriteString(nameof(RuleData.MatchExpression), instance.MatchExpression);
        if (instance.PathTemplate != null) jsonWriter.WriteString(nameof(RuleData.PathTemplate), instance.PathTemplate);

        return true;
    }
}
