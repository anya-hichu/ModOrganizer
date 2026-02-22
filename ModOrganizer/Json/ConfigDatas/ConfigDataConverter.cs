using Dalamud.Plugin.Services;
using ModOrganizer.Configs;
using ModOrganizer.Json.RuleDatas;
using ModOrganizer.Rules;
using ModOrganizer.Shared;

using System.Diagnostics.CodeAnalysis;

namespace ModOrganizer.Json.ConfigDatas;

public class ConfigDataConverter(IPluginLog pluginLog, IConverter<RuleData, Rule> ruleDataConverter) : Converter<ConfigData, Config>(pluginLog), IConverter<ConfigData, Config>
{
    public override bool TryConvert(ConfigData configData, [NotNullWhen(true)] out Config? config)
    {
        config = null;

        var version = (int)configData.Version;

        if (!ruleDataConverter.TryConvertMany(configData.Rules, out var rules))
        {
            PluginLog.Error($"Failed to convert one or many [{nameof(RuleData)}] to [{nameof(Rule)}]");
            return false;
        }

        config = new Config()
        {
            Version = version,
            Rules = rules
        };

        return true;
    }
}
