using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ModOrganizer.Mods;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace ModOrganizer.Backups;

public class BackupManager(ModInterop modInterop, IDalamudPluginInterface pluginInterface, IPluginLog pluginLog)
{
    private static readonly string FILE_NAME_FORMAT = "sortOrder.%s.json";

    private string SortOrderPath { get; init; } = modInterop.GetSortOrderPath();
    private string BackupFolderPath { get; init; } = pluginInterface.ConfigDirectory.FullName;
    private IPluginLog PluginLog { get; init; } = pluginLog;

    public bool TryCreate(BackupKind kind, [NotNullWhen(true)] out Backup? backup) 
    {
        backup = null;
        
        var createdAt = DateTimeOffset.UtcNow;
        var fileName = string.Format(FILE_NAME_FORMAT, createdAt.ToUnixTimeMilliseconds());

        if (!TryCopyFile(SortOrderPath, GetBackupPath(fileName), false)) return false;

        backup = new()
        {
            Kind = kind,
            CreatedAt = createdAt.Date,
            FileName = fileName
        };

        return true;
    }

    public bool TryRestore(Backup backup) => TryCopyFile(GetBackupPath(backup), SortOrderPath, true);

    public bool TryDelete(Backup backup)
    {
        try
        {
            File.Delete(GetBackupPath(backup));
            return true;
        }
        catch (Exception e)
        {
            PluginLog.Error($"Caught exception while try to delete [{backup.Kind}] backup [{backup.CreatedAt}] at [{backup.FileName}] ({e.Message})");
        }
        return false;
    }

    private bool TryCopyFile(string sourcePath, string destinationPath, bool overwrite)
    {
        try
        {
            File.Copy(sourcePath, destinationPath, overwrite);
            return true;
        }
        catch (Exception e)
        {
            PluginLog.Warning($"Caught exception while try to copy [{sourcePath}] to [{destinationPath}] with overwrite [{overwrite}] ({e.Message})");
        }
        return false;
    }

    private string GetBackupPath(Backup backup) => GetBackupPath(backup.FileName);
    private string GetBackupPath(string backupFileName) => Path.Combine(BackupFolderPath, backupFileName);
}
