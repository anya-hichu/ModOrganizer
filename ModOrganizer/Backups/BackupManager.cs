using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ModOrganizer.Mods;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace ModOrganizer.Backups;

public class BackupManager(Config config, ModInterop modInterop, IDalamudPluginInterface pluginInterface, IPluginLog pluginLog)
{
    private Config Config { get; init; } = config;
    private ModInterop ModInterop { get; init; } = modInterop;
    private IDalamudPluginInterface PluginInterface { get; init; } = pluginInterface;
    private IPluginLog PluginLog { get; init; } = pluginLog;

    public bool TryCreate([NotNullWhen(true)] out Backup? backup, bool manual = true) 
    {
        backup = null;
        
        var createdAt = DateTimeOffset.UtcNow;
        var fileName = GenerateBackupFileName(createdAt);

        if (!TryCopyFile(ModInterop.GetSortOrderPath(), GetBackupPath(fileName), false)) return false;

        backup = new()
        {
            Manual = manual,
            CreatedAt = createdAt,
            FileName = fileName
        };

        RegisterBackup(backup);
        return true;
    }

    public bool TryRestore(Backup backup) 
    {
        if (Config.AutoBackupEnabled)
        {
            if (!TryCreate(out var newBackup, manual: false))
            {
                PluginLog.Error("Failed to create security backup before restore, aborting");
                return false;
            }
            RegisterBackup(newBackup);
        }

        return TryCopyFile(GetBackupPath(backup), ModInterop.GetSortOrderPath(), true);
    } 

    public bool TryDelete(Backup backup)
    {
        try
        {
            File.Delete(GetBackupPath(backup));
            UnregisterBackup(backup);
            return true;
        }
        catch (Exception e)
        {
            PluginLog.Error($"Caught exception while try to delete backup [{backup.CreatedAt}] file [{backup.FileName}] ({e.Message})");
        }
        return false;
    }

    private void RegisterBackup(Backup backup)
    {
        Config.Backups.Add(backup);
        SaveConfig();
    }

    private void UnregisterBackup(Backup backup)
    {
        Config.Backups.Remove(backup);
        SaveConfig();
    }

    private void SaveConfig() => PluginInterface.SavePluginConfig(Config);

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

    public string GetBackupPath(Backup backup) => GetBackupPath(backup.FileName);

    private string GetBackupPath(string backupFileName) => Path.Combine(GetBackupsFolderPath(), backupFileName);

    public string GetBackupsFolderPath() => PluginInterface.ConfigDirectory.FullName;

    private static string GenerateBackupFileName(DateTimeOffset offset) => string.Concat("sort_order.", offset.ToUnixTimeMilliseconds(), ".json");
}
