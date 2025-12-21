using Dalamud.Plugin.Services;
using ModOrganizer.Backups;
using ModOrganizer.Configs;
using ModOrganizer.Rules;
using Penumbra.Api.Enums;
using System.Diagnostics.CodeAnalysis;

namespace ModOrganizer.Mods;

public class ModProcessor(BackupManager backupManager, Config config, ModInterop modInterop, IPluginLog pluginLog, RuleEvaluator ruleEvaluator)
{
    public bool TryProcess(string modDirectory, [NotNullWhen(true)] out string? newModPath, bool dryRun = false)
    {
        newModPath = null;

        if (!modInterop.TryGetModInfo(modDirectory, out var modInfo)) return false;
        if (ruleEvaluator.TryEvaluate(config.Rules, modInfo, out newModPath))
        {
            if (dryRun) return true;

            if (config.AutoBackupEnabled) backupManager.CreateRecent(manual: false);

            return modInterop.SetModPath(modInfo.Directory, newModPath) != PenumbraApiEc.PathRenameFailed;
        }
        pluginLog.Warning($"No rule matched mod [{modInfo.Directory}]");
        return false;
    }
}
