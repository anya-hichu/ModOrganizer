using Dalamud.Plugin.Services;
using ModOrganizer.Backups;
using ModOrganizer.Json.Files;
using ModOrganizer.Json.SortOrders;
using ModOrganizer.Mods;
using ModOrganizer.Windows.States.Results;
using ModOrganizer.Windows.States.Results.Backups;
using ModOrganizer.Windows.States.Results.Showables;
using System.Linq;

namespace ModOrganizer.Windows.States;

public class BackupState : ResultState, IShowableBackupResultState
{
    private BackupManager BackupManager { get; init; }
    private SortOrderBuilder SortOrderBuilder { get; init; }

    public bool ShowSamePaths { get; set; } = false;

    public string PathFilter { get; set; } = string.Empty;
    public string NewPathFilter { get; set; } = string.Empty;

    public Backup? Selected { get; private set; }


    public BackupState(BackupManager backupManager, ModInterop modInterop, IPluginLog pluginLog) : base(modInterop, pluginLog)
    {
        BackupManager = backupManager;
        ModInterop = modInterop;
        PluginLog = pluginLog;

        SortOrderBuilder = new(pluginLog);

        ModInterop.OnSortOrderChanged += Preview;
    }

    public new void Dispose()
    {
        base.Dispose();
        ModInterop.OnSortOrderChanged -= Preview;
    }

    public override void Clear()
    {
        base.Clear();
        Selected = null;

        PathFilter = string.Empty;
        NewPathFilter = string.Empty;
    }

    public void Select(Backup backup)
    {
        if (Selected == backup) return;

        Selected = backup;
        Preview();
    }

    public void Unselect() => Clear();

    public void Preview() => CancelAndRunTask(cancellationToken =>
    {
        if (Selected == null) return;
        if (!SortOrderBuilder.TryBuildFromFile(BackupManager.GetBackupPath(Selected), out var backupSortOrder)) return;

        var sortOrder = ModInterop.GetSortOrder();
        var allModDirectories = sortOrder.Data.Keys.Union(backupSortOrder.Data.Keys);

        Results.Clear();
        Results = [.. allModDirectories.Select<string, Result>(modDirectory => {
            cancellationToken.ThrowIfCancellationRequested();

            sortOrder.Data.TryGetValue(modDirectory, out var path);
            backupSortOrder.Data.TryGetValue(modDirectory, out var newPath);

            if (path == newPath) return new BackupSamePathResult() { Directory = modDirectory, Path = path };

            return new BackupPathResult() 
            { 
                Directory = modDirectory, 
                Path = path, 
                NewPath = newPath 
            };
        })];    
    });

    public void Apply()
    {
        if (Selected == null) return;

        if (!BackupManager.TryRestore(Selected)) PluginLog.Error($"Failed to restore backup from [{Selected.CreatedAt}]");
    }
}
