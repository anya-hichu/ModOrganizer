using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Clipboards;
using ModOrganizer.Json.Readers.Elements;
using ModOrganizer.Json.Readers.Files;
using ModOrganizer.Json.RuleDatas;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.ConfigDatas;

public class ConfigDataReader(IElementReader fileReader, IReader<RuleData> ruleDataReader, IPluginLog pluginLog) : Reader<ConfigData>(pluginLog), IClipboardReader<ConfigData>, IFileReader<ConfigData>
{
    private static readonly uint SUPPORTED_VERSION = 0;

    public IElementReader ElementReader { get; init; } = fileReader;

    public override bool TryRead(JsonElement jsonElement, [NotNullWhen(true)] out ConfigData? instance)
    {
        instance = null;

        if (!Assert.IsValue(jsonElement, JsonValueKind.Object)) return false;

        int? version = jsonElement.TryGetProperty(nameof(ConfigData.Version), out var versionProperty) ? versionProperty.GetInt32() : null;
        if (version != null && version != SUPPORTED_VERSION)
        {
            PluginLog.Warning($"Failed to read [{nameof(ConfigData)}], unsupported [{nameof(ConfigData.Version)}] found [{version}] (supported version: {SUPPORTED_VERSION}): {jsonElement}");
            return false;
        }

        RuleData[]? rules = null;
        if (Assert.IsPropertyPresent(jsonElement, nameof(ConfigData.Rules), out var rulesProperty) && !ruleDataReader.TryReadMany(rulesProperty, out rules))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(RuleData)}] for [{nameof(ConfigData)}]: {rulesProperty}");
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
