using Dalamud.Plugin;
using ModOrganizer.Shared;

namespace ModOrganizer.Configs;

public class ConfigLoader(IConfigDefault configDefault, IDalamudPluginInterface pluginInterface) : IConfigLoader
{
    public IConfig GetOrDefault() => pluginInterface.GetPluginConfig() as IConfig ?? configDefault.Build();
}
