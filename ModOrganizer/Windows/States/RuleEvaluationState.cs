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
    public HashSet<string> SelectedResultKeys { get; set; } = [];

    public bool ShowErrors { get; set; } = true;
    public bool ShowUnchanging { get; set; } = false;

    public Task Evaluate(IEnumerable<string> modDirectories) => RunTask(cancellationTokenSource =>
    {
        SelectedResultKeys.Clear();
        Results.Clear();

        var selectedResultKeys = new HashSet<string>();
        Results = modDirectories.ToDictionary(d => d, modDirectory =>
        {
            if (cancellationTokenSource.IsCancellationRequested) throw new TaskCanceledException($"Task [{Task.CurrentId}] has been canceled inside [{nameof(Evaluate)}] before processing mod [{modDirectory}]");

            try
            {
                if (!ModProcessor.TryProcess(modDirectory, out var newModPath, dryRun: true)) return new ArgumentException("No rule matched");
                selectedResultKeys.Add(modDirectory);
                return (object)newModPath;
            }
            catch (Exception e)
            {
                PluginLog.Warning($"Caught exception while evaluating mod [{modDirectory}] path:\n\t{e.Message}");
                return new ArgumentException("Failed to evaluate", e);
            }
        });
        SelectedResultKeys = selectedResultKeys;
    });

    public Task Apply() => RunTask(cancellationTokenSource =>
    {
        foreach (var modDirectory in SelectedResultKeys)
        {
            if (!Results.TryGetValue(modDirectory, out var result)) continue;
            if (cancellationTokenSource.IsCancellationRequested) throw new TaskCanceledException($"Task [{Task.CurrentId}] has been canceled inside [{nameof(Apply)}] before processing mod [{modDirectory}]");
            if (result is not string newModPath) continue;

            SelectedResultKeys.Remove(modDirectory);
            if (ModInterop.SetModPath(modDirectory, newModPath) == PenumbraApiEc.PathRenameFailed)
            {
                // Override path with error
                Results[modDirectory] = new ArgumentException("Failed to apply path", new ArgumentException($"Probably a conflict for path [{newModPath}]"));
                continue;
            }
            Results.Remove(modDirectory); 
        }
    });

    public override void Clear()
    {
        SelectedResultKeys.Clear();
        base.Clear();
    }

    public bool IsResultSelected(string key) => SelectedResultKeys.Contains(key);
    public bool SelectResult(string key, bool value) => value ? SelectedResultKeys.Add(key) : SelectedResultKeys.Remove(key);

    public void InvertResultSelection() => SelectedResultKeys = [.. Results.Keys.Except(SelectedResultKeys)];

    public void ClearResultSelection() => SelectedResultKeys.Clear();
}
