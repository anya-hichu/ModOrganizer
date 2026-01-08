using Dalamud.Plugin.Services;
using ModOrganizer.Mods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ModOrganizer.Windows.Results;

public abstract class ResultState : IDisposable
{
    protected IModInterop ModInterop { get; init; }
    protected IPluginLog PluginLog { get; init; }

    private Task CurrentTask { get; set; } = Task.CompletedTask;

    private CancellationTokenSource CancellationTokenSource { get; set; } = new();

    protected HashSet<Result> Results { get; set; } = [];

    public string DirectoryFilter { get; set; } = string.Empty;

    public ResultState(IModInterop modInterop, IPluginLog pluginLog)
    {
        ModInterop = modInterop;
        PluginLog = pluginLog;

        ModInterop.OnModDeleted += OnModDeleted;
        ModInterop.OnModMoved += OnModMoved;
    }

    public void Dispose()
    {
        CancelCurrentTask();

        ModInterop.OnModDeleted -= OnModDeleted;
        ModInterop.OnModMoved -= OnModMoved;
    }

    private void CancelCurrentTask() => CancellationTokenSource.Cancel();

    private void OnModDeleted(string modDirectory) => Results.Remove(new() { Directory = modDirectory });

    private void OnModMoved(string modDirectory, string newModDirectory)
    {
        if (!Results.TryGetValue(new() { Directory = modDirectory }, out var result)) return;

        Results.Remove(result);
        result.Directory = modDirectory;
        Results.Add(result);
    }

    public virtual void Clear()
    {
        CancelCurrentTask();
        Results.Clear();
    }

    protected Task CancelAndRunTask(Action<CancellationToken> action)
    {
        CancelCurrentTask();
        CancellationTokenSource = new();
        var cancellationToken = CancellationTokenSource.Token;
        return CurrentTask = CurrentTask.ContinueWith(_ => action(cancellationToken), cancellationToken);
    }

    public bool HasCompletedTask() => CurrentTask.IsCompleted;

    public IEnumerable<Result> GetResults() => Results;
    public IEnumerable<T> GetResults<T>() => Results.SelectMany<Result, T>(result => result is T castedResult ? [castedResult] : []);
}
