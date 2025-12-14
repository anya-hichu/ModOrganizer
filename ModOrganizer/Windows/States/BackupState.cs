using Dalamud.Plugin.Services;
using ModOrganizer.Backups;
using ModOrganizer.Json.Files;
using ModOrganizer.Json.SortOrders;
using ModOrganizer.Mods;
using ModOrganizer.Windows.States.Results;
using ModOrganizer.Windows.States.Results.Backups;
using ModOrganizer.Windows.States.Results.Showables;
using System.Collections.Generic;
using System.Linq;

namespace ModOrganizer.Windows.States;

public class BackupState : ResultState, IShowableBackupResultState
{
    private BackupManager BackupManager { get; init; }
    private SortOrderBuilder SortOrderBuilder { get; init; }

    public Backup? Selected { get; private set; }
    private SortOrder? MaybeSelectedSortOrderCache { get; set; }

    public bool ShowSamePaths { get; set; } = true;

    public string PathFilter { get; set; } = string.Empty;
    public string NewPathFilter { get; set; } = string.Empty;


    public BackupState(BackupManager backupManager, ModInterop modInterop, IPluginLog pluginLog) : base(modInterop, pluginLog)
    {
        BackupManager = backupManager;
        ModInterop = modInterop;
        PluginLog = pluginLog;

        SortOrderBuilder = new(pluginLog);

        ModInterop.OnSortOrderChanged += Evaluate;
    }

    public new void Dispose()
    {
        base.Dispose();
        ModInterop.OnSortOrderChanged -= Evaluate;
    }

    public void Select(Backup backup)
    {
        if (Selected == backup) return;

        Selected = backup;
        InvalidateSelectedSortOrderCache();
        Evaluate();
    }

    public void Deselect(Backup backup)
    {
        if (Selected == backup) Clear();
    }

    public override void Clear()
    {
        base.Clear();
        Selected = null;
        InvalidateSelectedSortOrderCache();
    }

    public void Evaluate() => CancelAndRunTask(cancellationToken =>
    {
        Results.Clear();

        var sortOrder = ModInterop.GetSortOrder();
        var backupSortOrder = GetBackupSortOrder();
        Results = [.. GetAllOrderDataModDirectories().Select<string, Result>(modDirectory => {
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

    public void Restore()
    {
        if (Selected == null) return;

        if (!BackupManager.TryRestore(Selected)) PluginLog.Error($"Failed to restore backup from [{Selected.CreatedAt}]");
    }

    private void InvalidateSelectedSortOrderCache() 
    {
        PluginLog.Debug($"Invalidating backup sort order data cache [count: {MaybeSelectedSortOrderCache?.Data?.Count}]");
        MaybeSelectedSortOrderCache = null;
    } 

    private SortOrder GetBackupSortOrder()
    {
        if (MaybeSelectedSortOrderCache != null) return MaybeSelectedSortOrderCache;

        if (Selected != null && SortOrderBuilder.TryBuildFromFile(BackupManager.GetBackupPath(Selected), out var sortOrder))
        {
            MaybeSelectedSortOrderCache = sortOrder;
            PluginLog.Debug($"Loaded [{nameof(SortOrder)}] cache (count: {sortOrder.Data.Count})");
        }
        else
        {
            MaybeSelectedSortOrderCache = new() { Data = [], EmptyFolders = [] };
            PluginLog.Warning($"Failed to parse [{nameof(SortOrder)}], cached empty until next selection or reload");
        }

        return MaybeSelectedSortOrderCache;
    }


    private HashSet<string> GetAllOrderDataModDirectories()
    {
        var modDirectories = ModInterop.GetSortOrder().Data.Keys.ToHashSet();
        if (Selected == null) return modDirectories;

        var backupModDirectories = GetBackupSortOrder().Data.Keys;
        return [.. modDirectories.Union(backupModDirectories)];
    }
}
