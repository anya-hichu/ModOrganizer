using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ModOrganizer.Backups;
using ModOrganizer.Rules;
using ModOrganizer.Shared;
using Penumbra.Api.Enums;
using System;
using System.Diagnostics.CodeAnalysis;


namespace ModOrganizer.Mods;

public class ModProcessor(ActionDebouncer actionDebouncer, BackupManager backupManager, Config config, ModInterop modInterop, IDalamudPluginInterface pluginInterface, IPluginLog pluginLog, RuleEvaluator ruleEvaluator)
{
    public bool TryProcess(string modDirectory, [NotNullWhen(true)] out string? newModPath, bool dryRun = false)
    {
        newModPath = null;
        if (!modInterop.TryGetModInfo(modDirectory, out var modInfo)) return false;
        if (ruleEvaluator.TryEvaluateByPriority(config.Rules, modInfo, out newModPath))
        {
            if (dryRun) return true;

            if (config.AutoBackupEnabled) actionDebouncer.Invoke(nameof(CreateAutoBackup), CreateAutoBackup, TimeSpan.FromMicroseconds(config.AutoBackupWaitMs), leading: true, trailing: false);

            return modInterop.SetModPath(modInfo.Directory, newModPath) != PenumbraApiEc.PathRenameFailed;
        }
        pluginLog.Warning($"No rule matched mod [{modInfo.Directory}]");
        return false;
    }

    private void CreateAutoBackup()
    {
        var kind = BackupKind.Auto;
        if (!backupManager.TryCreate(kind, out var backup))
        {
            pluginLog.Error($"Failed to create [{kind}] backup");
            return;
        }
        config.Backups.Add(backup);
        pluginInterface.SavePluginConfig(config);

        pluginLog.Info($"Successfully created [{kind}] backup [{backup.CreatedAt}] as file [{backup.FileName}]");
    }
}
