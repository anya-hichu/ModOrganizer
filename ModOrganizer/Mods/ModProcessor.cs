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

    public bool TryProcess(string modDirectory, [NotNullWhen(true)] out string? path)
    {
        path = default;
        if (ModInterop.TryGetModInfo(modDirectory, out var modInfo) && RuleEvaluator.TryEvaluateByPriority(Config.Rules, modInfo, out path))
        {
            var exitCode = ModInterop.SetModPath(modDirectory, path);
            if (exitCode == PenumbraApiEc.Success)
            {
                PluginLog.Info($"Set mod [{modDirectory}] path to [{path}]");
                return true;
            }
            PluginLog.Error($"Failed to set mod [{modDirectory}] path to [{path}] ({exitCode})");
        }
        return false;
    }
}
