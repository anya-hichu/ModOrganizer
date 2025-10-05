using Dalamud.Plugin.Services;
using Dalamud.Utility;
using ModOrganizer.Mods;
using ModOrganizer.Utils;
using Scriban;
using System.Collections.Generic;
using System.Linq;


namespace ModOrganizer.Rules;

public class RuleEvaluator(IPluginLog pluginLog)
{
    private IPluginLog PluginLog { get; init; } = pluginLog;

    public bool TryEvaluateChain(IEnumerable<Rule> rules, ModInfo modInfo, out string? path)
    {
        path = default;
        foreach (var rule in rules.OrderByDescending(r => r.Priority))
        {
            if (TryEvaluate(rule, modInfo, out path))
            {
                return true;
            }
        }
        return false;
    }

    private bool TryEvaluate(Rule rule, ModInfo modInfo, out string? path)
    {
        path = default;
        if (!rule.Enabled || rule.PathTemplate.IsNullOrWhitespace())
        {
            return false;
        }

        if (!Matches(rule, modInfo))
        {
            return false;
        }

        var template = Template.Parse(rule.PathTemplate);
        var result = template.Render(modInfo, ScribanUtils.RenameMember);
        if (result.IsNullOrWhitespace())
        {
            return false;
        }

        path = result;
        return true;
    }

    private bool Matches(Rule rule, ModInfo modInfo)
    {
        if (rule.MatchExpression.IsNullOrWhitespace())
        {
            return false;
        }

        var result = Template.Evaluate(rule.MatchExpression, modInfo, ScribanUtils.RenameMember);
        if (result is bool validResult)
        {
            return validResult;
        }

        PluginLog.Error($"Match expression [{rule.MatchExpression}] did not evaluate to a boolean, ignoring");
        return false;
    }
}
