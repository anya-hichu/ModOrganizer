using Dalamud.Plugin.Services;
using ModOrganizer.Mods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ModOrganizer.Windows.States.Results;

public abstract class ResultState : IDisposable
{
    protected ModInterop ModInterop { get; init; }
    protected IPluginLog PluginLog { get; init; }

    private Task EvaluationTask { get; set; } = Task.CompletedTask;
    private CancellationTokenSource CancellationTokenSource { get; set; } = new();

    protected Dictionary<string, Result> ResultByModDirectory { get; set; } = [];

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

    private void OnModDeleted(string modDirectory) => ResultByModDirectory.Remove(modDirectory);

    private void OnModMoved(string modDirectory, string newModDirectory)
    {
        if (!ResultByModDirectory.TryGetValue(modDirectory, out var result)) return;

        ResultByModDirectory.Remove(modDirectory);
        ResultByModDirectory.Add(newModDirectory, result);
    }

    public virtual void Clear()
    {
        CancelTask();
        ResultByModDirectory.Clear();
    }

    protected Task CancelAndRunTask(Action<CancellationToken> action)
    {
        CancelTask();
        CancellationTokenSource = new();
        var cancellationToken = CancellationTokenSource.Token;
        return EvaluationTask = EvaluationTask.ContinueWith(_ => action(cancellationToken), cancellationToken);
    }

    public IReadOnlyDictionary<string, Result> GetResultByModDirectory() => ResultByModDirectory;
    public IReadOnlyDictionary<string, T> GetResultByModDirectory<T>() => ResultByModDirectory.SelectMany<KeyValuePair<string, Result>, KeyValuePair<string, T>>(p => p.Value is T casted ? [new(p.Key, casted)] : []).ToDictionary();
}
