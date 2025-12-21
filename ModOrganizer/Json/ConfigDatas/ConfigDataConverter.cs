using Dalamud.Plugin.Services;
using ModOrganizer.Configs;
using ModOrganizer.Json.RuleDatas;
using ModOrganizer.Shared;
using System.Diagnostics.CodeAnalysis;

namespace ModOrganizer.Json.ConfigDatas;

public class ConfigDataConverter(IPluginLog pluginLog) : Converter<ConfigData, Config>(pluginLog)
{
    private RuleDataConverter RuleDataConverter { get; init; } = new(pluginLog);

    public override bool TryConvert(ConfigData configData, [NotNullWhen(true)] out Config? config)
    {
        config = new();

        if (configData.Version.HasValue) config.Version = configData.Version.Value;
        if (configData.Rules != null && RuleDataConverter.TryConvertMany(configData.Rules, out var rules)) config.Rules = [.. rules];

        return true;
    }
}
