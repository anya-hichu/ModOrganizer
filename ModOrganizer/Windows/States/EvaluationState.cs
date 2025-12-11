using Dalamud.Plugin.Services;
using Dalamud.Utility;
using ModOrganizer.Mods;
using ModOrganizer.Rules;
using ModOrganizer.Shared;
using ModOrganizer.Windows.States.Results;
using ModOrganizer.Windows.States.Results.Showables;
using Scriban;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
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

            if (!ModInterop.TryGetModInfo(modDirectory, out var modInfo)) return new ErrorResult() { Message = "Failed to retrieve mod info" };

            // Need to keep context for string conversion
            var templateContext = new TemplateContext() { MemberRenamer = MemberRenamer.Rename };

            var scriptObject = new ScriptObject();
            scriptObject.Import(modInfo);
            templateContext.PushGlobal(scriptObject);

            try
            {
                var expressionResult = Scriban.Template.Evaluate(Expression, templateContext);
                var templateResult = Scriban.Template.Parse(Template).Render(templateContext);
                return new EvaluationResult() 
                { 
                    ExpressionValue = templateContext.ObjectToString(expressionResult) , 
                    TemplateValue = templateResult 
                };
            }
            catch (Exception e)
            {
                PluginLog.Warning($"Caught exception while evaluating for mod [{modDirectory}]:\n\t{e.Message}");
                return new ErrorResult()
                {
                    Message = "Failed to evaluate",
                    InnerMessage = e.Message
                };
            }
        });
    });

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
