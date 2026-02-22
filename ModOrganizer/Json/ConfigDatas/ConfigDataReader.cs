using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Elements;
using ModOrganizer.Json.RuleDatas;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.ConfigDatas;

public class ConfigDataReader(IElementReader elementReader, IReader<RuleData> ruleDataReader, IPluginLog pluginLog) : Reader<ConfigData>(pluginLog), IConfigDataReader
{
    public static readonly uint SUPPORTED_VERSION = 0;

    public IElementReader ElementReader { get; init; } = elementReader;

    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out ConfigData? instance)
    {
        instance = null;

        if (!element.Is(JsonValueKind.Object, PluginLog)) return false;

        if (!element.TryGetRequiredPropertyValue(nameof(ConfigData.Version), out uint version, PluginLog)) return false;

        if (version != SUPPORTED_VERSION)
        {
            PluginLog.Warning($"Failed to read [{nameof(ConfigData)}], unsupported [{nameof(ConfigData.Version)}] found [{version}] (supported version: {SUPPORTED_VERSION}): {element}");
            return false;
        }

        if (!element.TryGetRequiredProperty(nameof(ConfigData.Rules), out var rulesProperty, PluginLog)) return false;

        if (!ruleDataReader.TryReadMany(rulesProperty, out var rules))
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
