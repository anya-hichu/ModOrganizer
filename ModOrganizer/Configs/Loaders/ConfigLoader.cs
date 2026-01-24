using Dalamud.Plugin;
using ModOrganizer.Configs.Defaults;

namespace ModOrganizer.Configs.Loaders;

public class ConfigLoader(IConfigDefault configDefault, IDalamudPluginInterface pluginInterface) : IConfigLoader
{
    public IConfig GetOrDefault() => pluginInterface.GetPluginConfig() as IConfig ?? configDefault.Build();
}
