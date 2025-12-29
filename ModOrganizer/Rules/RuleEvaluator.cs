using Dalamud.Plugin.Services;
using Dalamud.Utility;
using ModOrganizer.Mods;
using ModOrganizer.Shared;
using Scriban;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ModOrganizer.Rules;

public class RuleEvaluator(IPluginLog pluginLog) : IRuleEvaluator
{
    public bool TryEvaluateMany(IEnumerable<Rule> rules, ModInfo modInfo, [NotNullWhen(true)] out string? path)
    {
        path = null;
        foreach (var rule in rules.OrderDescending())
        {
            if (TryEvaluate(rule, modInfo, out path))
            {
                pluginLog.Debug($"Rule [{rule.Path}] matched mod [{modInfo.Directory}] and evaluated to path [{path}]");
                return true;
            }
        }
        pluginLog.Debug($"No rule matched mod [{modInfo.Directory}] with [{rules.Where(r => r.Enabled).Count()}] rules enabled out of [{rules.Count()}]");
        return false;
    }

    public bool TryEvaluate(Rule rule, ModInfo modInfo, [NotNullWhen(true)] out string? path)
    {
        path = null;
        if (!rule.Enabled || rule.PathTemplate.IsNullOrWhitespace() || !Matches(rule, modInfo)) return false;

        var template = Template.Parse(rule.PathTemplate);
        if (template.HasErrors)
        {
            pluginLog.Error($"Failed to parse path template [{rule.PathTemplate}], ignoring: {template.Messages}");
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

        try
        {
            var result = Template.Evaluate(rule.MatchExpression, modInfo, MemberRenamer.Rename);
            if (result is bool boolResult) return boolResult;

            pluginLog.Error($"Match expression [{rule.MatchExpression}] did not evaluate to a boolean, ignoring");
        } 
        catch (Exception e)
        {
            pluginLog.Error($"Caught expression while evaluating match expression [{rule.MatchExpression}] ({e.Message}), ignoring");
        }
        
        return false;
    }
}
