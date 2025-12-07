using Dalamud.Plugin.Services;
using ModOrganizer.Mods;
using ModOrganizer.Scriban;
using Scriban;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ModOrganizer.Windows.States;

public class EvaluationState(ModInterop modInterop, IPluginLog pluginLog) : IDisposable
{
    private ModInterop ModInterop { get; init; } = modInterop;
    private IPluginLog PluginLog { get; init; } = pluginLog;
    private Task EvaluationTask { get; set; } = Task.CompletedTask;
    private CancellationTokenSource CancellationTokenSource { get; set; } = new();

    public string Expression { get; set; } = string.Empty;
    public string ModDirectoryFilter { get; set; } = string.Empty;
    public string ResultFilter { get; set; } = string.Empty;

    public Dictionary<string, object> Results { get; set; } = [];

    public void Dispose() => CancelPrevious();

    public Task EvaluateAsync(IEnumerable<string> modDirectories)
    {
        CancelPrevious();
        var source = CancellationTokenSource = new();
        return EvaluationTask = EvaluationTask.ContinueWith(_ =>
        {
            Results.Clear();
            Results = modDirectories.ToDictionary(d => d, modDirectory =>
            {
                if (source.IsCancellationRequested) throw new TaskCanceledException($"Task [{Task.CurrentId}] has been canceled inside [{nameof(EvaluateAsync)}] before processing mod [{modDirectory}]");
                if (!ModInterop.TryGetModInfo(modDirectory, out var modInfo)) return new ArgumentException("Failed to retrieve mod info");

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

    private void CancelPrevious() => CancellationTokenSource.Cancel();

    public void Clear()
    {
        CancelPrevious();
        Results.Clear();
        Expression = string.Empty;
        ModDirectoryFilter = string.Empty;
        ResultFilter = string.Empty;
    }
}
