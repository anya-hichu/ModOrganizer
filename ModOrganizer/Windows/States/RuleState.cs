using Dalamud.Plugin.Services;
using ModOrganizer.Backups;
using ModOrganizer.Configs;
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

public class RuleState(IConfig config, IBackupManager backupManager, IModInterop modInterop, IModProcessor modProcessor, IPluginLog pluginLog) : ResultState(modInterop, pluginLog), ISelectableResultState, IShowableRuleResultState
{
    public event Action? OnResultsChanged;

    public bool ShowErrors { get; set; } = true;
    public bool ShowSamePaths { get; set; } = false;

    public string PathFilter { get; set; } = string.Empty;
    public string NewPathFilter { get; set; } = string.Empty;

    public Task Preview(HashSet<string> modDirectories) => CancelAndRunTask(cancellationToken =>
    {
        Results.Clear();
        OnResultsChanged?.Invoke();

        Results = [.. modDirectories.Select<string, Result>(modDirectory =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            var currentModPath = ModInterop.GetModPath(modDirectory);
            try
            {
                if (!modProcessor.TryProcess(modDirectory, out var newModPath, dryRun: true)) return new RuleErrorResult() { Directory = modDirectory, Path = currentModPath, Message = "No rule matched" };
                if (currentModPath == newModPath) return new RuleSamePathResult() { Directory = modDirectory, Path = currentModPath };

                return new RulePathResult()
                {
                    Directory = modDirectory,
                    Path = currentModPath, 
                    NewPath = newModPath
                };
            }
            catch (Exception e)
            {
                PluginLog.Warning($"Caught exception while evaluating mod [{modDirectory}] path:\n\t{e.Message}");
                return new RuleErrorResult()
                {
                    Directory = modDirectory,
                    Path = currentModPath, 
                    Message = "Failed to evaluate", 
                    InnerMessage = e.Message
                };
            }
        })];
        OnResultsChanged?.Invoke();
    });

    public Task Apply() => CancelAndRunTask(cancellationToken =>
    {
        Results = [.. Results.SelectMany<Result, Result>(result =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (result is not RulePathResult rulePathResult) return [result];
            if (!rulePathResult.Selected) return [result];

            if (config.AutoBackupEnabled) backupManager.CreateRecent(auto: false);

            var newModPath = rulePathResult.NewPath;
            if (ModInterop.SetModPath(rulePathResult.Directory, newModPath) == PenumbraApiEc.PathRenameFailed)
            {
                var conflictingModDirectory = ModInterop.GetModDirectory(newModPath);
                return [new RuleErrorResult()
                {
                    Directory = rulePathResult.Directory,
                    Path = rulePathResult.Path,
                    Message = $"Failed to apply [{newModPath}]",
                    InnerMessage = $"Conflicts with mod [{conflictingModDirectory}]"
                }];
            }

            return [];
        })];
        OnResultsChanged?.Invoke();
    });

    public override void Clear()
    {
        base.Clear();
        DirectoryFilter = string.Empty;
        PathFilter = string.Empty;
        NewPathFilter = string.Empty;
    }
}
