using Dalamud.Plugin.Services;
using Dalamud.Utility;
using ModOrganizer.Mods;
using ModOrganizer.Utils;
using Scriban;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;


namespace ModOrganizer.Rules;

public class RuleEvaluator(IPluginLog pluginLog)
{
    private IPluginLog PluginLog { get; init; } = pluginLog;

    public bool TryEvaluateByPriority(IEnumerable<Rule> rules, ModInfo modInfo, [NotNullWhen(true)] out string? path)
    {
        path = null;
        foreach (var rule in rules.OrderByDescending(r => r.Priority))
        {
            if (TryEvaluate(rule, modInfo, out path))
            {
                PluginLog.Debug($"Rule [{rule.Name}] matched mod [{modInfo.Directory}] and evaluated to path [{path}]");
                return true;
            }
        }
        PluginLog.Debug($"No rule matched mod [{modInfo.Directory}]");
        return false;
    }

    public bool TryEvaluate(Rule rule, ModInfo modInfo, [NotNullWhen(true)] out string? path)
    {
        path = null;
        if (!rule.Enabled || rule.PathTemplate.IsNullOrWhitespace() || !Matches(rule, modInfo)) return false;

        var template = Template.Parse(rule.PathTemplate);
        if (template.HasErrors)
        {
            PluginLog.Error($"Failed to parse rule [{rule.Name}] path template, ignoring:\n\t{template.Messages}");
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
