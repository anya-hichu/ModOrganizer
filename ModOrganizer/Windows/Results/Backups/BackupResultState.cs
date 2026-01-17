using Dalamud.Plugin.Services;
using ModOrganizer.Backups;
using ModOrganizer.Mods;
using System.Linq;
using System.Threading.Tasks;

namespace ModOrganizer.Windows.Results.Backups;

public class BackupResultState : ResultState, IBackupResultState
{
    private IBackupManager BackupManager { get; init; }

    public bool ShowSamePaths { get; set; } = false;

    public string PathFilter { get; set; } = string.Empty;
    public string OldPathFilter { get; set; } = string.Empty;

    private Backup? Selected { get; set; }

    public bool ReloadPenumbra { get; set; } = true;


    public BackupResultState(IBackupManager backupManager, IModInterop modInterop, IPluginLog pluginLog) : base(modInterop, pluginLog)
    {
        BackupManager = backupManager;
        ModInterop = modInterop;
        PluginLog = pluginLog;

        ModInterop.OnSortOrderChanged += OnSortOrderChanged;
    }

    public new void Dispose()
    {
        base.Dispose();
        ModInterop.OnSortOrderChanged -= OnSortOrderChanged;
    }

    public override void Clear()
    {
        base.Clear();
        Selected = null;

        PathFilter = string.Empty;
        OldPathFilter = string.Empty;
    }

    public bool IsSelected(Backup backup) => Selected == backup;

    public void Select(Backup backup)
    {
        if (Selected == backup) return;

        Selected = backup;
        Preview();
    }

    public void Unselect() => Clear();

    public Task Preview() => CancelAndRunTask(cancellationToken =>
    {
        if (Selected == null) return;
        if (!BackupManager.TryRead(Selected, out var backupSortOrder)) return;

        var sortOrder = ModInterop.GetSortOrder();
        var allModDirectories = sortOrder.Data.Keys.Union(backupSortOrder.Data.Keys);

        Results.Clear();
        Results = [.. allModDirectories.Select((System.Func<string, Result>)(modDirectory => {
            cancellationToken.ThrowIfCancellationRequested();

            sortOrder.Data.TryGetValue(modDirectory, out var path);
            backupSortOrder.Data.TryGetValue(modDirectory, out var oldPath);

            if (path == oldPath) return new BackupSamePathResult() { Directory = modDirectory, Path = path };

            return new BackupPathResult() 
            { 
                Directory = modDirectory, 
                Path = path, 
                OldPath = oldPath
            };
        }))];    
    });

    public void Apply()
    {
        if (Selected == null) return;

        if (!BackupManager.TryRestore(Selected, ReloadPenumbra)) PluginLog.Error($"Failed to restore backup from [{Selected.CreatedAt}]");
    }

    private void OnSortOrderChanged() => Preview();
}
