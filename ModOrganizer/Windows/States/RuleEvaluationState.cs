using Dalamud.Plugin.Services;
using ModOrganizer.Mods;
using ModOrganizer.Windows.States.Results;
using ModOrganizer.Windows.States.Results.Rules;
using ModOrganizer.Windows.States.Results.Selectables;
using ModOrganizer.Windows.States.Results.Visibles;
using Penumbra.Api.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModOrganizer.Windows.States;

public class RuleEvaluationState(ModInterop modInterop, ModProcessor modProcessor, IPluginLog pluginLog) : ResultState(modInterop, pluginLog), ISelectableResultState, IVisibleResultState
{
    public event Action? OnResultsChanged;

    private ModProcessor ModProcessor { get; init; } = modProcessor;

    public bool ShowErrors { get; set; } = true;
    public bool ShowUnchanging { get; set; } = false;

    public Task Evaluate(HashSet<string> modDirectories) => CancelAndRunTask(cancellationToken =>
    {
        ResultByModDirectory.Clear();
        OnResultsChanged?.Invoke();

        ResultByModDirectory = modDirectories.ToDictionary<string, string, Result>(d => d, modDirectory =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            var currentPath = ModInterop.GetModPath(modDirectory);
            try
            {
                if (!ModProcessor.TryProcess(modDirectory, out var newModPath, dryRun: true)) return new RuleErrorResult(currentPath, "No rule matched");
                if (currentPath == newModPath) return new RuleSamePathResult(currentPath);

                return new RulePathResult(currentPath, newModPath);
            }
            catch (Exception e)
            {
                PluginLog.Warning($"Caught exception while evaluating mod [{modDirectory}] path:\n\t{e.Message}");
                return new RuleErrorResult(currentPath, "Failed to evaluate", e.Message);
            }
        });
        OnResultsChanged?.Invoke();
    });

    public Task Apply() => CancelAndRunTask(cancellationToken =>
    {
        foreach (var (modDirectory, selectedResult) in this.GetSelectedResultByModDirectory())
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (selectedResult is not RulePathResult rulePathResult) continue;

            var newModPath = rulePathResult.NewPath;
            if (ModInterop.SetModPath(modDirectory, newModPath) == PenumbraApiEc.PathRenameFailed)
            {
                var conflictingModDirectory = ModInterop.GetModDirectory(newModPath);
                ResultByModDirectory[modDirectory] = new RuleErrorResult(rulePathResult.CurrentPath, $"Failed to apply [{newModPath}]", $"Conflicts with mod [{conflictingModDirectory}]");
                continue;
            }
            ResultByModDirectory.Remove(modDirectory); 
        }
        OnResultsChanged?.Invoke();
    });
}
