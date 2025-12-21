using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ModOrganizer.Configs;
using ModOrganizer.Mods;
using ModOrganizer.Shared;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using ThrottleDebounce;

namespace ModOrganizer.Backups;

public class BackupManager : IDisposable
{
    private IClock Clock { get; init; }
    private IConfig Config { get; init; }
    private IModInterop ModInterop { get; init; }
    private IDalamudPluginInterface PluginInterface { get; init; }
    private IPluginLog PluginLog { get; init; }

    private RateLimitedAction<bool> CreateRecentAction { get; init; }

    public BackupManager(IClock clock, IConfig config, IModInterop modInterop, IDalamudPluginInterface pluginInterface, IPluginLog pluginLog)
    {
        Clock = clock;
        Config = config;
        ModInterop = modInterop;
        PluginInterface = pluginInterface;
        PluginLog = pluginLog;

        CreateRecentAction = Debouncer.Debounce<bool>(Create, TimeSpan.FromSeconds(5), leading: true, trailing: false);
    }

    public void Dispose() => CreateRecentAction.Dispose();

    public bool TryCreate([NotNullWhen(true)] out Backup? backup, bool manual = true) 
    {
        backup = null;
        
        var createdAt = Clock.GetNowUtc();
        if (!TryCopyFile(ModInterop.GetSortOrderPath(), GetPath(createdAt))) return false;

        backup = new()
        {
            Manual = manual,
            CreatedAt = createdAt
        };

        return Register(backup);
    }

    public void Create(bool manual = true)
    {
        if (!TryCreate(out var newBackup, manual))
        {
            var message = $"Failed to create {(manual ? "manual" : "auto")} backup";
            PluginLog.Error(message);
            throw new BackupCreationException(message);
        }

        PluginLog.Debug($"Successfully created backup [{newBackup.CreatedAt}] file");
    }

    public void CreateRecent(bool manual = true) => CreateRecentAction.Invoke(manual);

    public bool TryRestore(Backup backup, bool reloadPenumbra = false) 
    {
        if (Config.AutoBackupEnabled)
        {
            if (!TryCreate(out var autoBackup, manual: false))
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
                PluginLog.Warning($"Failed to delete backup [{backup.CreatedAt}] file since it does not exists, ignoring");
            }
            return Unregister(backup);
        }
        catch (Exception e)
        {
            PluginLog.Error($"Caught exception while try to delete backup [{backup.CreatedAt}] file ({e.Message})");
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
            PluginLog.Warning("Failed to unregister backup from config, ignoring");
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
            PluginLog.Error($"Caught exception while try to copy [{sourcePath}] to [{destinationPath}] with overwrite [{overwrite}] ({e.Message})");
        }
        return false;
    }

    private void EnforceLimit()
    {
        var orderedAutoBackups = Config.Backups.Where(b => !b.Manual).OrderDescending();
        foreach (var oldAutoBackup in orderedAutoBackups.Skip(Convert.ToInt32(Config.AutoBackupLimit)))
        {
            if (!TryDelete(oldAutoBackup)) PluginLog.Debug("Failed to properly delete auto backup to enforce limit, ignoring");
        }
    }

    public string GetPath(Backup backup) => GetPath(backup.CreatedAt);

    private string GetPath(DateTimeOffset offset) => Path.Combine(GetFolderPath(), GetFileName(offset));

    public string GetFolderPath() => PluginInterface.ConfigDirectory.FullName;

    public static string GetFileName(DateTimeOffset offset) => string.Concat("sort_order.", offset.ToUnixTimeMilliseconds(), ".json");

    private void SaveConfig() => PluginInterface.SavePluginConfig(Config);
}
