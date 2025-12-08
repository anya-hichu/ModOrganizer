using Dalamud.Plugin.Services;
using ModOrganizer.Mods;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ModOrganizer.Windows.States;

public abstract class ResultState : IDisposable
{
    protected ModInterop ModInterop { get; init; }
    protected IPluginLog PluginLog { get; init; }

    private Task EvaluationTask { get; set; } = Task.CompletedTask;
    private CancellationTokenSource CancellationTokenSource { get; set; } = new();

    protected Dictionary<string, object> Results { get; set; } = [];

    public ResultState(ModInterop modInterop, IPluginLog pluginLog)
    {
        ModInterop = modInterop;
        PluginLog = pluginLog;

        ModInterop.OnModDeleted += OnModDeleted;
        ModInterop.OnModMoved += OnModMoved;
    }

    public void Dispose()
    {
        CancelTask();

        ModInterop.OnModDeleted -= OnModDeleted;
        ModInterop.OnModMoved -= OnModMoved;
    }

    private void CancelTask() => CancellationTokenSource.Cancel();

    private void OnModDeleted(string modDirectory) => Results.Remove(modDirectory);

    private void OnModMoved(string modDirectory, string newModDirectory)
    {
        if (Results.TryGetValue(modDirectory, out var result))
        {
            Results.Remove(modDirectory);
            Results.Add(newModDirectory, result);
        }
    }

    public virtual void Clear()
    {
        CancelTask();
        Results.Clear();
    }

    protected Task Run(Action<CancellationTokenSource> action)
    {
        CancelTask();
        var cancellationTokenSource = CancellationTokenSource = new();
        return EvaluationTask = EvaluationTask.ContinueWith(_ => action(cancellationTokenSource), cancellationTokenSource.Token);
    }

    public IReadOnlyDictionary<string, object> GetResults() => Results;
}
