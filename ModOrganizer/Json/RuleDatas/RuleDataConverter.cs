using Dalamud.Plugin.Services;
using ModOrganizer.Rules;
using ModOrganizer.Shared;
using System.Diagnostics.CodeAnalysis;

namespace ModOrganizer.Json.RuleDatas;

public class RuleDataConverter(IPluginLog pluginLog) : Converter<RuleData, Rule>(pluginLog), IConverter<RuleData, Rule>
{
    public override bool TryConvert(RuleData ruleData, [NotNullWhen(true)] out Rule? rule)
    {
        rule = new() { Path = ruleData.Path };

        if (ruleData.Enabled.HasValue) rule.Enabled = ruleData.Enabled.Value;
        if (ruleData.Priority.HasValue) rule.Priority = ruleData.Priority.Value;
        if (ruleData.MatchExpression != null) rule.MatchExpression = ruleData.MatchExpression;
        if (ruleData.PathTemplate != null) rule.PathTemplate = ruleData.PathTemplate;

        return true;
    }
}
