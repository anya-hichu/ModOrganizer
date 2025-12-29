using Dalamud.Plugin.Services;
using ModOrganizer.Configs;
using ModOrganizer.Json.RuleDatas;
using ModOrganizer.Rules;
using ModOrganizer.Shared;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ModOrganizer.Json.ConfigDatas;

public class ConfigDataConverter(IPluginLog pluginLog) : Converter<ConfigData, Config>(pluginLog)
{
    private RuleDataConverter RuleDataConverter { get; init; } = new(pluginLog);

    public override bool TryConvert(ConfigData configData, [NotNullWhen(true)] out Config? config)
    {
        config = null;

        var convertedConfig = new Config();
        if (configData.Version.HasValue) convertedConfig.Version = configData.Version.Value;

        var rules = new HashSet<Rule>();
        if (configData.Rules != null && !RuleDataConverter.TryConvertMany(configData.Rules, out rules))
        {
            PluginLog.Error($"Failed to convert [{nameof(ConfigData)}] to [{nameof(Config)}]");
            return false;
        }
        convertedConfig.Rules = rules;

        config = convertedConfig;
        return true;
    }
}
