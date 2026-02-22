using Dalamud.Plugin.Services;
using ModOrganizer.Json.RuleDatas;
using ModOrganizer.Json.Writers;
using System.Text.Json;

namespace ModOrganizer.Json.ConfigDatas;

public class ConfigDataWriter(IWriter<RuleData> ruleDataWriter, IPluginLog pluginLog) : Writer<ConfigData>(pluginLog), IConfigDataWriter
{
    public override bool TryWrite(Utf8JsonWriter jsonWriter, ConfigData instance)
    {
        using var _ = jsonWriter.WriteObject();

        jsonWriter.WriteNumber(nameof(ConfigData.Version), instance.Version);

        jsonWriter.WritePropertyName(nameof(ConfigData.Rules));
        if (!ruleDataWriter.TryWriteMany(jsonWriter, instance.Rules)) return false;

        return true;
    }
}
