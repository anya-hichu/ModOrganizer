using Dalamud.Plugin.Services;
using ModOrganizer.Rules;
using ModOrganizer.Shared;
using System.Diagnostics.CodeAnalysis;

namespace ModOrganizer.Json.RuleDatas;

public class RuleDataConverter(IPluginLog pluginLog) : Converter<RuleData, Rule>(pluginLog), IConverter<RuleData, Rule>
{
    public override bool TryConvert(RuleData ruleData, [NotNullWhen(true)] out Rule? rule)
    {
        rule = new()
        {
            Enabled = ruleData.Enabled,
            Path = ruleData.Path,
            Priority = ruleData.Priority,
            MatchExpression = ruleData.MatchExpression,
            PathTemplate = ruleData.PathTemplate
        };

        return true;
    }
}
