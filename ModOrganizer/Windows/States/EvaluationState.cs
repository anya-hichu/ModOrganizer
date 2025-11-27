using Dalamud.Plugin.Services;
using ModOrganizer.Mods;
using ModOrganizer.Scriban;
using Scriban;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModOrganizer.Windows.States;

public class EvaluationState(ModInterop modInterop, IPluginLog pluginLog)
{
    private ModInterop ModInterop { get; init; } = modInterop;
    private IPluginLog PluginLog { get; init; } = pluginLog;
    private Task EvaluationTask { get; set; } = Task.CompletedTask;

    public string Expression { get; set; } = string.Empty;
    public string ModDirectoryFilter { get; set; } = string.Empty;
    public string ResultFilter { get; set; } = string.Empty;

    public Dictionary<string, object> Results { get; set; } = [];

    public Task EvaluateAsync(IEnumerable<string> modDirectories)
    {
        return EvaluationTask = EvaluationTask.ContinueWith(_ =>
        {
            Results.Clear();
            Results = modDirectories.ToDictionary(d => d, modDirectory =>
            {
                if (!ModInterop.TryGetModInfo(modDirectory, out var modInfo)) return new ArgumentException("Failed to retrieve mod data");

                var templateContext = new TemplateContext() { MemberRenamer = MemberRenamer.Rename };

                var scriptObject = new ScriptObject();
                scriptObject.Import(modInfo);
                templateContext.PushGlobal(scriptObject);

                try
                {
                    var result = Template.Evaluate(Expression, templateContext);
                    return (object)templateContext.ObjectToString(result);
                }
                catch (Exception e)
                {
                    PluginLog.Warning($"Failed to evaluate expression [{Expression}] for mod [{modDirectory}]:\n\t{e.Message}");
                    return new ArgumentException("Failed to evaluate", e);
                }
            });
        });
    }

    public void Clear()
    {
        Results.Clear();
        Expression = string.Empty;
        ModDirectoryFilter = string.Empty;
        ResultFilter = string.Empty;
    }
}
