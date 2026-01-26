using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ModOrganizer.Rules;
using System.Collections.Generic;
using System.Linq;

namespace ModOrganizer.Configs.Mergers;

public class ConfigMerger(IConfig config, IDalamudPluginInterface pluginInterface, IPluginLog pluginLog) : IConfigMerger
{
    public IEnumerable<Rule> GetConflicts(IConfig otherConfig) => config.Rules.Intersect(otherConfig.Rules);

    public void Merge(IConfig otherConfig, bool overwrite)
    {
        var beforeCount = config.Rules.Count;
        if (overwrite) config.Rules.ExceptWith(otherConfig.Rules);

        config.Rules.UnionWith(otherConfig.Rules);
        var afterCount = config.Rules.Count;
        
        pluginInterface.SavePluginConfig(config);
        pluginLog.Debug($"Saved merged config rules (before: {beforeCount}, after: {afterCount}) with overwrite [{overwrite}]");
    }
}
