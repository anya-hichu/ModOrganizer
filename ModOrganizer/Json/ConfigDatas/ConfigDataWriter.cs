using Dalamud.Plugin.Services;
using ModOrganizer.Json.RuleDatas;
using ModOrganizer.Json.Writers;
using ModOrganizer.Json.Writers.Files;
using System.Text.Json;

namespace ModOrganizer.Json.ConfigDatas;

public class ConfigDataWriter(IPluginLog pluginLog) : Writer<ConfigData>(pluginLog), IFileWriter<ConfigData>
{
    private RuleDataWriter RuleDataWriter { get; init; } = new(pluginLog);

    public override bool TryWrite(Utf8JsonWriter jsonWriter, ConfigData instance)
    {
        using var _ = jsonWriter.WriteObject();

        jsonWriter.WriteNumber(nameof(ConfigData.Version), instance.Version);
        if (!RuleDataWriter.TryWriteMany(jsonWriter, instance.Rules)) return false;

        return true;
    }
}
