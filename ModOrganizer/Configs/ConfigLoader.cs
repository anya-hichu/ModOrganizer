using Dalamud.Plugin;

namespace ModOrganizer.Configs;

public class ConfigLoader(IDalamudPluginInterface pluginInterface)
{
    public Config GetOrDefault() => pluginInterface.GetPluginConfig() as Config ?? new ConfigDefault(new()).Build();
}
