using Dalamud.Plugin.Services;
using ModOrganizer.Rules;
using Penumbra.Api.Enums;

namespace ModOrganizer.Mods;

public class ModProcessor(Config config, ModInterop modInterop, IPluginLog PluginLog, RuleEvaluator ruleEvaluator)
{
    private Config Config { get; init; } = config;
    private ModInterop ModInterop { get; init; } = modInterop;
    private IPluginLog PluginLog { get; init; } = PluginLog;
    private RuleEvaluator RuleEvaluator { get; init; } = ruleEvaluator;

    public bool TryProcess(string modDirectory, out string? newModDirectory)
    {
        var modInfo = ModInterop.GetCachedModInfo(modDirectory);
        if (RuleEvaluator.TryEvaluateChain(Config.Rules, modInfo, out newModDirectory) && modDirectory != newModDirectory)
        {
            var exitCode = ModInterop.SetModPath(modDirectory, newModDirectory!);
            if (exitCode == PenumbraApiEc.Success)
            {
                ModInterop.ClearCaches(modDirectory);
                PluginLog.Info($"Moved mod [{modDirectory}] to [{newModDirectory}]");
                return true;
            }
            PluginLog.Error($"Failed to move mod [{modDirectory}] to [{newModDirectory}], exit code [{exitCode}]");
        }
        return false;
    }
}
