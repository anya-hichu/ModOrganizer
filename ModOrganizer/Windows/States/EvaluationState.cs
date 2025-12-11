using Dalamud.Plugin.Services;
using Dalamud.Utility;
using ModOrganizer.Mods;
using ModOrganizer.Rules;
using ModOrganizer.Windows.States.Results;
using ModOrganizer.Windows.States.Results.Showables;
using Scriban;
using Scriban.Parsing;
using Scriban.Syntax;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace ModOrganizer.Windows.States;

public class EvaluationState(ModInterop modInterop, IPluginLog pluginLog) : ResultState(modInterop, pluginLog), IShowableEvaluationResultState
{
    public string Expression { get; set; } = string.Empty;
    public string Template { get; set; } = string.Empty;

    public string ModDirectoryFilter { get; set; } = string.Empty;
    public string ExpressionFilter { get; set; } = string.Empty;
    public string TemplateFilter { get; set; } = string.Empty;

    public Task Evaluate(HashSet<string> modDirectories) => CancelAndRunTask(cancellationToken =>
    {
        ResultByModDirectory.Clear();
        ResultByModDirectory = modDirectories.ToDictionary<string, string, Result>(d => d, modDirectory =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            var evaluationResult = new EvaluationResult();
            if (!ModInterop.TryGetModInfo(modDirectory, out var modInfo)) 
            {
                evaluationResult.ExpressionError = evaluationResult.TemplateError = new() { Message = "Failed to retrieve mod info" };
                return evaluationResult;
            }
             
            if (!TryEvaluate(modInfo, Expression, out var expressionValue, out var expressionError, ScriptMode.ScriptOnly)) PluginLog.Warning($"Failed to evaluate [{Expression}] for mod [{modDirectory}]");
            evaluationResult.ExpressionValue = expressionValue;
            evaluationResult.ExpressionError = expressionError;

            if (!TryEvaluate(modInfo, Template, out var templateValue, out var templateError)) PluginLog.Warning($"Failed to evaluate [{Template}] for mod [{modDirectory}]");
            evaluationResult.TemplateValue = templateValue;
            evaluationResult.TemplateError = templateError;

            return evaluationResult;
        });
    });

    private bool TryEvaluate(ModInfo modInfo, string template, [NotNullWhen(true)] out string? value, [NotNullWhen(false)] out Error? error, ScriptMode scriptMode = ScriptMode.Default)
    {
        value = null;
        error = null;

        var parsedTemplate = Scriban.Template.Parse(template, lexerOptions: new() { Mode = scriptMode });
        if (parsedTemplate.HasErrors)
        {
            error = new()
            {
                Message = "Failed to parse template",
                InnerMessage = parsedTemplate.Messages.ToString()
            };
            return false;
        }
        else
        {
            try
            {
                value = parsedTemplate.Render(modInfo);
                return true;
            }
            catch (ScriptRuntimeException e)
            {
                PluginLog.Debug($"Caught exception while evaluating [{template}] for mod [{modInfo.Directory}]:\n\t{e.Message}");
                error = new()
                {
                    Message = "Failed to evaluate template",
                    InnerMessage = e.Message
                };

                return false;
            }
        }
    }

    public override void Clear()
    {
        base.Clear();
        Expression = string.Empty;
        Template = string.Empty;
        ModDirectoryFilter = string.Empty;
        ExpressionFilter = string.Empty;
    }

    public void Load(Rule rule)
    {
        Expression = rule.MatchExpression;
        Template = rule.PathTemplate;
    }

    public bool HasFilters() => !ModDirectoryFilter.IsNullOrWhitespace() || !ExpressionFilter.IsNullOrWhitespace() || !TemplateFilter.IsNullOrWhitespace();


}
