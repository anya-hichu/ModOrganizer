using Dalamud.Plugin.Services;
using ModOrganizer.Mods;
using ModOrganizer.Windows.States.Results;
using ModOrganizer.Windows.States.Results.Rules;
using ModOrganizer.Windows.States.Results.Selectables;
using ModOrganizer.Windows.States.Results.Showables;
using Penumbra.Api.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModOrganizer.Windows.States;

public class RuleEvaluationState(ModInterop modInterop, ModProcessor modProcessor, IPluginLog pluginLog) : ResultState(modInterop, pluginLog), ISelectableResultState, IShowableRuleResultState
{
    public event Action? OnResultsChanged;

    private ModProcessor ModProcessor { get; init; } = modProcessor;

    public bool ShowErrors { get; set; } = true;
    public bool ShowSamePaths { get; set; } = false;

    public Task Evaluate(HashSet<string> modDirectories) => CancelAndRunTask(cancellationToken =>
    {
        ResultByModDirectory.Clear();
        OnResultsChanged?.Invoke();

        ResultByModDirectory = modDirectories.ToDictionary<string, string, Result>(d => d, modDirectory =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            var currentModPath = ModInterop.GetModPath(modDirectory);
            try
            {
                if (!ModProcessor.TryProcess(modDirectory, out var newModPath, dryRun: true)) return new RuleErrorResult() { CurrentPath = currentModPath, Message = "No rule matched" };
                if (currentModPath == newModPath) return new RuleSamePathResult() { CurrentPath = currentModPath };

                return new RulePathResult()
                {
                    CurrentPath = currentModPath, 
                    NewPath = newModPath
                };
            }
            catch (Exception e)
            {
                PluginLog.Warning($"Caught exception while evaluating mod [{modDirectory}] path:\n\t{e.Message}");
                return new RuleErrorResult()
                {
                    CurrentPath = currentModPath, 
                    Message = "Failed to evaluate", 
                    InnerMessage = e.Message
                };
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
                ResultByModDirectory[modDirectory] = new RuleErrorResult()
                {
                    CurrentPath = rulePathResult.CurrentPath,
                    Message = $"Failed to apply [{newModPath}]",
                    InnerMessage = $"Conflicts with mod [{conflictingModDirectory}]"
                };
                continue;
            }
            ResultByModDirectory.Remove(modDirectory); 
        }
        OnResultsChanged?.Invoke();
    });
}
