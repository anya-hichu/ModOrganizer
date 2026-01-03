using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ModOrganizer.Configs;
using ModOrganizer.Json.Penumbra.SortOrders;
using ModOrganizer.Json.Readers.Files;
using ModOrganizer.Mods;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using ThrottleDebounce;

namespace ModOrganizer.Backups;

public class BackupManager : IBackupManager
{
    private IConfig Config { get; init; }
    private IModInterop ModInterop { get; init; }
    private IDalamudPluginInterface PluginInterface { get; init; }
    private IPluginLog PluginLog { get; init; }
    private IFileReader<SortOrder> SortOrderFileReader { get; init; }

    private RateLimitedAction<bool> CreateRecentAction { get; init; }

    public BackupManager(IConfig config, IModInterop modInterop, IDalamudPluginInterface pluginInterface, IPluginLog pluginLog, IFileReader<SortOrder> sortOrderFileReader)
    {
        Config = config;
        ModInterop = modInterop;
        PluginInterface = pluginInterface;
        PluginLog = pluginLog;
        SortOrderFileReader = sortOrderFileReader;

        CreateRecentAction = Debouncer.Debounce<bool>(auto => Create(auto), TimeSpan.FromSeconds(5), leading: true, trailing: false);
    }

    public void Dispose() => CreateRecentAction.Dispose();

    public Backup Create(bool auto = false)
    {
        if (!TryCreate(out var newBackup, auto))
        {
            var message = $"Failed to create {Backup.GetPrettyType(auto)} backup";
            PluginLog.Error(message);
            throw new BackupCreationException(message);
        }

        return newBackup;
    }

    public bool TryCreate([NotNullWhen(true)] out Backup? backup, bool auto = false) 
    {
        backup = null;
        
        var createdAt = DateTimeOffset.UtcNow;
        if (!TryCopyFile(ModInterop.GetSortOrderPath(), GetPath(createdAt))) return false;

        backup = new()
        {
            CreatedAt = createdAt,
            Auto = auto
        };

        return Register(backup);
    }

    public void CreateRecent(bool auto = false) => CreateRecentAction.Invoke(auto);

    public bool TryRestore(Backup backup, bool reloadPenumbra = false) 
    {
        if (Config.AutoBackupEnabled)
        {
            if (!TryCreate(out var autoBackup, auto: true))
            {
                PluginLog.Error("Failed to create auto backup before restore, aborting");
                return false;
            }
            PluginLog.Debug($"Created auto backup [{autoBackup.CreatedAt}] file before restore");
        }

        if (!TryCopyFile(GetPath(backup), ModInterop.GetSortOrderPath(), overwrite: true)) return false;

        if (reloadPenumbra && !ModInterop.ReloadPenumbra()) PluginLog.Error("Failed to reload penumbra after restoring backup");

        return true;
    } 

    public bool TryDelete(Backup backup)
    {
        try
        {
            var path = GetPath(backup);
            if (File.Exists(path))
            {
                File.Delete(path);
            } 
            else
            {
                PluginLog.Warning($"Failed to delete {backup.GetPrettyType()} backup [{backup.CreatedAt}] file since it does not exists, ignoring");
            }
            return Unregister(backup);
        }
        catch (Exception e)
        {
            PluginLog.Error($"Caught exception while trying to delete {backup.GetPrettyType()} backup [{backup.CreatedAt}] file ({e.Message})");
        }
        return false;
    }

    private bool Register(Backup backup)
    {
        if (!Config.Backups.Add(backup))
        {
            PluginLog.Warning("Failed to register backup in config, ignoring");
            return true;
        }

        EnforceLimit();
        SaveConfig();
        return true;
    }

    private bool Unregister(Backup backup)
    {
        if (!Config.Backups.Remove(backup))
        {
            PluginLog.Warning($"Failed to unregister {backup.GetPrettyType()} backup from config, ignoring");
            return true;
        }

        SaveConfig();
        return true;
    }

    private bool TryCopyFile(string sourcePath, string destinationPath, bool overwrite = false)
    {
        try
        {
            File.Copy(sourcePath, destinationPath, overwrite);
            return true;
        }
        catch (Exception e)
        {
            PluginLog.Error($"Caught exception while trying to copy [{sourcePath}] to [{destinationPath}] with overwrite [{overwrite}] ({e.Message})");
        }
        return false;
    }

    private void EnforceLimit()
    {
        var orderedAutoBackups = Config.Backups.Where(b => b.Auto).OrderDescending();
        foreach (var oldAutoBackup in orderedAutoBackups.Skip(Convert.ToInt32(Config.AutoBackupLimit)))
        {
            if (!TryDelete(oldAutoBackup)) PluginLog.Debug("Failed to properly delete auto backup to enforce limit, ignoring");
        }
    }

    public bool TryRead(Backup backup, [NotNullWhen(true)] out SortOrder? sortOrder) => SortOrderFileReader.TryReadFromFile(GetPath(backup), out sortOrder);

    public string GetPath(Backup backup) => GetPath(backup.CreatedAt);
    private string GetPath(DateTimeOffset offset) => Path.Combine(GetFolderPath(), GetFileName(offset));

    public string GetFolderPath() => PluginInterface.ConfigDirectory.FullName;

    public string GetFileName(Backup backup) => GetFileName(backup.CreatedAt);
    private static string GetFileName(DateTimeOffset offset) => string.Concat("sort_order.", offset.ToUnixTimeMilliseconds(), ".json");

    private void SaveConfig() => PluginInterface.SavePluginConfig(Config);
}
