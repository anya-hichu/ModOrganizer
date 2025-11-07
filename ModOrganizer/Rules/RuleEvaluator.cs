using Dalamud.Plugin.Services;
using Dalamud.Utility;
using ModOrganizer.Mods;
using ModOrganizer.Scripts;
using Scriban;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;


namespace ModOrganizer.Rules;

public class RuleEvaluator(IPluginLog pluginLog)
{
    private IPluginLog PluginLog { get; init; } = pluginLog;

    public bool TryEvaluateChain(IEnumerable<Rule> rules, ModInfo modInfo, [NotNullWhen(true)] out string? path)
    {
        path = default;
        foreach (var rule in rules.OrderByDescending(r => r.Priority))
        {
            if (TryEvaluate(rule, modInfo, out path)) return true;
        }
        return false;
    }

    private bool TryEvaluate(Rule rule, ModInfo modInfo, [NotNullWhen(true)] out string? path)
    {
        path = default;
        if (!rule.Enabled || rule.PathTemplate.IsNullOrWhitespace() || !Matches(rule, modInfo)) return false;

        var template = Template.Parse(rule.PathTemplate);
        if (template.HasErrors)
        {
            PluginLog.Error($"Failed to parse rule [{rule.Name}] path template: {template.Messages}");
            return false;
        }
        var result = template.Render(modInfo, MemberRenamer.Rename);
        if (result.IsNullOrWhitespace()) return false;

        path = result;
        return true;
    }

    private bool Matches(Rule rule, ModInfo modInfo)
    {
        if (rule.MatchExpression.IsNullOrWhitespace()) return false;

        var result = Template.Evaluate(rule.MatchExpression, modInfo, MemberRenamer.Rename);
        if (result is bool validResult) return validResult;

        PluginLog.Error($"Match expression [{rule.MatchExpression}] did not evaluate to a boolean, ignoring");
        return false;
    }
}
