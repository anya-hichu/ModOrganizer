using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Clipboards;
using ModOrganizer.Json.Readers.Elements;
using ModOrganizer.Json.Readers.Files;
using ModOrganizer.Json.RuleDatas;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.ConfigDatas;

public class ConfigDataReader(IElementReader elementReader, IReader<RuleData> ruleDataReader, IPluginLog pluginLog) : Reader<ConfigData>(pluginLog), IClipboardReader<ConfigData>, IFileReader<ConfigData>
{
    private static readonly uint SUPPORTED_VERSION = 0;

    public IElementReader ElementReader { get; init; } = elementReader;

    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out ConfigData? instance)
    {
        instance = null;

        if (!element.Is(JsonValueKind.Object, PluginLog)) return false;

        int? version = element.TryGetProperty(nameof(ConfigData.Version), out var versionProperty) ? versionProperty.GetInt32() : null;
        if (version != null && version != SUPPORTED_VERSION)
        {
            PluginLog.Warning($"Failed to read [{nameof(ConfigData)}], unsupported [{nameof(ConfigData.Version)}] found [{version}] (supported version: {SUPPORTED_VERSION}): {element}");
            return false;
        }

        RuleData[]? rules = null;
        if (element.TryGetRequiredProperty(nameof(ConfigData.Rules), out var rulesProperty, PluginLog) && !ruleDataReader.TryReadMany(rulesProperty, out rules))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(RuleData)}] for [{nameof(ConfigData)}]: {element}");
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
