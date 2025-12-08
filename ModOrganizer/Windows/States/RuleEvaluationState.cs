using Dalamud.Plugin.Services;
using ModOrganizer.Mods;
using Penumbra.Api.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModOrganizer.Windows.States;

public class RuleEvaluationState(ModInterop modInterop, ModProcessor modProcessor, IPluginLog pluginLog) : ResultState(modInterop, pluginLog)
{
    private ModProcessor ModProcessor { get; init; } = modProcessor;

    public Task Evaluate(IEnumerable<string> modDirectories) => RunTask(cancellationTokenSource =>
    {
        Results.Clear();
        Results = modDirectories.ToDictionary(d => d, modDirectory =>
        {
            if (cancellationTokenSource.IsCancellationRequested) throw new TaskCanceledException($"Task [{Task.CurrentId}] has been canceled inside [{nameof(Evaluate)}] before processing mod [{modDirectory}]");
            try
            {
                if (!ModProcessor.TryProcess(modDirectory, out var newModPath, dryRun: true)) return new ArgumentException("No rule matched");
                return (object)newModPath;
            }
            catch (Exception e)
            {
                PluginLog.Warning($"Caught exception while evaluating mod [{modDirectory}] path:\n\t{e.Message}");
                return new ArgumentException("Failed to evaluate", e);
            }
        });
    });

    public Task Apply() => RunTask(cancellationTokenSource =>
    {
        foreach (var (modDirectory, result) in Results)
        {
            if (cancellationTokenSource.IsCancellationRequested) throw new TaskCanceledException($"Task [{Task.CurrentId}] has been canceled inside [{nameof(Apply)}] before processing mod [{modDirectory}]");
            if (result is not string newModPath) continue;

            if (ModInterop.SetModPath(modDirectory, newModPath) == PenumbraApiEc.PathRenameFailed)
            {
                // Override path with error
                Results[modDirectory] = new ArgumentException("Failed to apply path due to conflict");
                continue;
            }
            Results.Remove(modDirectory);
        }
    });
}
