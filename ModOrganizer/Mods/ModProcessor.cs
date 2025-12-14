using Dalamud.Plugin.Services;
using ModOrganizer.Backups;
using ModOrganizer.Rules;
using ModOrganizer.Shared;
using Penumbra.Api.Enums;
using System;
using System.Diagnostics.CodeAnalysis;

namespace ModOrganizer.Mods;

public class ModProcessor(ActionDebouncer actionDebouncer, BackupManager backupManager, Config config, ModInterop modInterop, IPluginLog pluginLog, RuleEvaluator ruleEvaluator)
{
    public bool TryProcess(string modDirectory, [NotNullWhen(true)] out string? newModPath, bool dryRun = false)
    {
        newModPath = null;
        if (!modInterop.TryGetModInfo(modDirectory, out var modInfo)) return false;
        if (ruleEvaluator.TryEvaluate(config.Rules, modInfo, out newModPath))
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
        if (!backupManager.TryCreate(out var backup, manual: false))
        {
            pluginLog.Error($"Failed to create auto backup");
            return;
        }

        pluginLog.Debug($"Successfully created auto backup [{backup.CreatedAt}] with file [{backup.FileName}]");
    }
}
