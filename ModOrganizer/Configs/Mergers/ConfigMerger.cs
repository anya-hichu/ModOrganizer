using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using System.Linq;

namespace ModOrganizer.Configs.Mergers;

public class ConfigMerger(IConfig config, IDalamudPluginInterface pluginInterface, IPluginLog pluginLog) : IConfigMerger
{
    public int CountConflicts(IConfig newConfig) => config.Rules.Intersect(newConfig.Rules).Count();

    public void Merge(IConfig partialConfig, bool overwrite)
    {
        var beforeCount = config.Rules.Count;
        if (overwrite) config.Rules.ExceptWith(partialConfig.Rules);

        config.Rules.UnionWith(partialConfig.Rules);
        var afterCount = config.Rules.Count;

        pluginLog.Debug($"Merged config (rules before: {beforeCount}, after: {afterCount}) with overwrite [{overwrite}]");
        pluginInterface.SavePluginConfig(config);
    }
}
