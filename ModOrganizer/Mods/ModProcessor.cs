using Dalamud.Plugin.Services;
using ModOrganizer.Rules;
using Penumbra.Api.Enums;
using System.Diagnostics.CodeAnalysis;


namespace ModOrganizer.Mods;

public class ModProcessor(Config config, ModInterop modInterop, IPluginLog pluginLog, RuleEvaluator ruleEvaluator)
{
    private Config Config { get; init; } = config;
    private ModInterop ModInterop { get; init; } = modInterop;
    private IPluginLog PluginLog { get; init; } = pluginLog;
    private RuleEvaluator RuleEvaluator { get; init; } = ruleEvaluator;

    public bool TryProcess(string modDirectory, [NotNullWhen(true)] out string? path, bool dryRun = false)
    {
        path = null;
        if (!ModInterop.TryGetModInfo(modDirectory, out var modInfo)) return false;
        return TryProcess(modInfo, out path, dryRun);
    }

    public bool TryProcess(ModInfo modInfo, [NotNullWhen(true)] out string? path, bool dryRun = false)
    {
        if (RuleEvaluator.TryEvaluateByPriority(Config.Rules, modInfo, out path))
        {
            if (dryRun) return true;
            return ModInterop.SetModPath(modInfo.Directory, path) != PenumbraApiEc.PathRenameFailed;
        }
        PluginLog.Warning($"No rule matched mod [{modInfo.Directory}]");
        return false;
    }
}
