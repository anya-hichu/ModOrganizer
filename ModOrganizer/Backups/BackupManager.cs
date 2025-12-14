using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ModOrganizer.Mods;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace ModOrganizer.Backups;

public class BackupManager(Config config, ModInterop modInterop, IDalamudPluginInterface pluginInterface, IPluginLog pluginLog)
{
    private static readonly string FILE_NAME_FORMAT = "sort_order.%s.json";

    private Config Config { get; init; } = config;
    private ModInterop ModInterop { get; init; } = modInterop;
    private IDalamudPluginInterface PluginInterface { get; init; } = pluginInterface;
    private IPluginLog PluginLog { get; init; } = pluginLog;

    public bool TryCreate([NotNullWhen(true)] out Backup? backup, bool manual = true) 
    {
        backup = null;
        
        var createdAt = DateTimeOffset.UtcNow;
        var fileName = string.Format(FILE_NAME_FORMAT, createdAt.ToUnixTimeMilliseconds());

        if (!TryCopyFile(ModInterop.GetSortOrderPath(), GetBackupPath(fileName), false)) return false;

        backup = new()
        {
            Manual = manual,
            CreatedAt = createdAt.Date,
            FileName = fileName
        };

        Config.Backups.Add(backup);
        PluginInterface.SavePluginConfig(Config);

        return true;
    }

    public bool TryRestore(Backup backup) => TryCopyFile(GetBackupPath(backup), ModInterop.GetSortOrderPath(), true);

    public bool TryDelete(Backup backup)
    {
        try
        {
            File.Delete(GetBackupPath(backup));

            Config.Backups.Add(backup);
            PluginInterface.SavePluginConfig(Config);

            return true;
        }
        catch (Exception e)
        {
            PluginLog.Error($"Caught exception while try to delete backup [{backup.CreatedAt}] file [{backup.FileName}] ({e.Message})");
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

    public string GetBackupPath(Backup backup) => GetBackupPath(backup.FileName);

    private string GetBackupPath(string backupFileName) => Path.Combine(GetBackupsFolderPath(), backupFileName);

    public string GetBackupsFolderPath() => PluginInterface.ConfigDirectory.FullName;
}
