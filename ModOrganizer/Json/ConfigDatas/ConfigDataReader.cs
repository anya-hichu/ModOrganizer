using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Clipboards;
using ModOrganizer.Json.Readers.Files;
using ModOrganizer.Json.RuleDatas;
using ModOrganizer.Json.RuleExports;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.ConfigDatas;

public class ConfigDataReader(IPluginLog pluginLog) : Reader<ConfigData>(pluginLog), IReadableClipboard<ConfigData>, IReadableFile<ConfigData>
{
    private static readonly uint SUPPORTED_VERSION = 0;

    public ClipboardReader ClipboardReader { get; init; } = new(pluginLog);
    public FileReader FileReader { get; init; } = new(pluginLog);

    public RuleDataReader RuleDataReader { get; init; } = new(pluginLog);

    public override bool TryRead(JsonElement jsonElement, [NotNullWhen(true)] out ConfigData? instance)
    {
        instance = null;

        if (!Assert.IsValue(jsonElement, JsonValueKind.Object)) return false;

        int? version = jsonElement.TryGetProperty(nameof(ConfigData.Version), out var versionProperty) ? versionProperty.GetInt32() : null;
        if (version != null && version != SUPPORTED_VERSION)
        {
            PluginLog.Warning($"Failed to read [{nameof(ConfigData)}], unsupported [{nameof(ConfigData.Version)}] found [{version}] (supported version: {SUPPORTED_VERSION}):\n\t{jsonElement}");
            return false;
        }

        RuleData[]? rules = null;
        if (Assert.IsPropertyPresent(jsonElement, nameof(ConfigData.Rules), out var rulesProperty) && !RuleDataReader.TryReadMany(rulesProperty, out rules))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(RuleData)}] for [{nameof(ConfigData)}]:\n\t{rulesProperty}");
            return false;
        }

        instance = new()
        {
            Version = version,
            Rules = rules
        };

        return true;
    }
}
