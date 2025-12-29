using Dalamud.Plugin.Services;
using ModOrganizer.Backups;
using ModOrganizer.Configs;
using ModOrganizer.Rules;
using Penumbra.Api.Enums;
using System.Diagnostics.CodeAnalysis;

namespace ModOrganizer.Mods;

public class ModProcessor(IBackupManager backupManager, IConfig config, IModInterop modInterop, IPluginLog pluginLog, IRuleEvaluator ruleEvaluator) : IModProcessor
{
    public bool TryProcess(string modDirectory, [NotNullWhen(true)] out string? newModPath, bool dryRun = false)
    {
        newModPath = null;

        if (!modInterop.TryGetModInfo(modDirectory, out var modInfo)) return false;
        if (ruleEvaluator.TryEvaluateMany(config.Rules, modInfo, out newModPath))
        {
            if (dryRun) return true;

            if (config.AutoBackupEnabled) backupManager.CreateRecent(auto: true);

            return modInterop.SetModPath(modInfo.Directory, newModPath) != PenumbraApiEc.PathRenameFailed;
        }
        pluginLog.Warning($"No rule matched mod [{modInfo.Directory}]");
        return false;
    }
}
