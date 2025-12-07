using Dalamud.Plugin.Services;
using ModOrganizer.Mods;
using ModOrganizer.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ModOrganizer.Windows.States;

public class RuleEvaluationState(ModInterop modInterop, IPluginLog pluginLog, RuleEvaluator ruleEvaluator) : IDisposable
{
    private ModInterop ModInterop { get; init; } = modInterop;
    private RuleEvaluator RuleEvaluator { get; init; } = ruleEvaluator;
    private IPluginLog PluginLog { get; init; } = pluginLog;

    private Task EvaluationTask { get; set; } = Task.CompletedTask;
    private CancellationTokenSource CancellationTokenSource { get; set; } = new();
    public Dictionary<string, string?> Results { get; set; } = [];

    public void Dispose() => CancelPrevious();

    public Task PreviewAsync(IEnumerable<Rule> rules, IEnumerable<string> modDirectories)
    {
        CancelPrevious();
        var source = CancellationTokenSource = new();
        return EvaluationTask = EvaluationTask.ContinueWith(_ =>
        {
            Results.Clear();
            Results = modDirectories.ToDictionary(d => d, modDirectory =>
            {
                if (source.IsCancellationRequested) throw new TaskCanceledException($"Task [{Task.CurrentId}] has been canceled inside [{nameof(PreviewAsync)}] before processing mod [{modDirectory}]");
                if (!ModInterop.TryGetModInfo(modDirectory, out var modInfo)) return null;
                if (!RuleEvaluator.TryEvaluateByPriority(rules, modInfo, out var path)) PluginLog.Warning($"Evaluated mod [{modDirectory}] path as [null] since no rule matched");
                return path;
            });
        }, source.Token);
    }

    public Task ApplyAsync()
    {
        CancelPrevious();
        var source = CancellationTokenSource = new();
        return EvaluationTask = EvaluationTask.ContinueWith(_ =>
        {
            foreach (var (modDirectory, maybeNewModPath) in Results)
            {
                if (source.IsCancellationRequested) throw new TaskCanceledException($"Task [{Task.CurrentId}] has been canceled inside [{nameof(ApplyAsync)}] before processing mod [{modDirectory}]");
                if (maybeNewModPath == null)
                {
                    PluginLog.Debug($"Ignore mod [{modDirectory}] path since it evaluated to [null]");
                    continue;
                }
                ModInterop.SetModPath(modDirectory, maybeNewModPath);
            }
            Results.Clear();
        }, source.Token);
    }

    private void CancelPrevious() => CancellationTokenSource.Cancel();

    public void Clear()
    {
        CancelPrevious();
        Results.Clear();
    }
}
