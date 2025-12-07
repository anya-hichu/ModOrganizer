using Dalamud.Plugin.Services;
using ModOrganizer.Mods;
using Penumbra.Api.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ModOrganizer.Windows.States;

public class RuleEvaluationState(ModInterop modInterop, IPluginLog pluginLog, ModProcessor modProcessor) : IDisposable
{
    private ModInterop ModInterop { get; init; } = modInterop;
    private ModProcessor ModProcessor { get; init; } = modProcessor;
    private IPluginLog PluginLog { get; init; } = pluginLog;

    private Task EvaluationTask { get; set; } = Task.CompletedTask;
    private CancellationTokenSource CancellationTokenSource { get; set; } = new();
    public Dictionary<ModInfo, object?> Results { get; set; } = [];

    public void Dispose() => CancelPrevious();

    public Task EvaluateAsync(IEnumerable<string> modDirectories)
    {
        CancelPrevious();
        var source = CancellationTokenSource = new();
        return EvaluationTask = EvaluationTask.ContinueWith(_ =>
        {
            Results.Clear();
            var results = new Dictionary<ModInfo, object?>();
            foreach (var modDirectory in modDirectories)
            {
                if (source.IsCancellationRequested) throw new TaskCanceledException($"Task [{Task.CurrentId}] has been canceled inside [{nameof(EvaluateAsync)}] before processing mod [{modDirectory}]");
                if (!ModInterop.TryGetModInfo(modDirectory, out var modInfo))
                {
                    PluginLog.Warning($"Failed to retrive mod [{modDirectory}] info");
                    continue;
                }
                if (!ModProcessor.TryProcess(modInfo, out var maybePath, dryRun: true)) PluginLog.Warning($"Failed to evaluate mod [{modDirectory}] path, using [null] as result");
                results.Add(modInfo, maybePath);
            }
            Results = results;
        }, source.Token);
    }

    public Task ApplyAsync()
    {
        CancelPrevious();
        var source = CancellationTokenSource = new();
        return EvaluationTask = EvaluationTask.ContinueWith(_ =>
        {
            foreach (var (modInfo, result) in Results)
            {
                if (source.IsCancellationRequested) throw new TaskCanceledException($"Task [{Task.CurrentId}] has been canceled inside [{nameof(ApplyAsync)}] before processing mod [{modInfo.Directory}]");
                if (result is Exception e) continue;
                
                var exitCode = ModInterop.SetModPath(modInfo.Directory, result as string);
                if (exitCode == PenumbraApiEc.PathRenameFailed)
                {
                    Results[modInfo] = new ArgumentException("Failed to set path due to conflict");
                    continue;
                }
                Results.Remove(modInfo);
            }
        }, source.Token);
    }

    private void CancelPrevious() => CancellationTokenSource.Cancel();

    public void Clear()
    {
        CancelPrevious();
        Results.Clear();
    }
}
