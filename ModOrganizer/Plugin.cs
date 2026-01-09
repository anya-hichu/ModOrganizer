using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using ModOrganizer.Commands;
using ModOrganizer.Mods;
using ModOrganizer.Windows;
using ModOrganizer.Windows.Configs;

namespace ModOrganizer;

public sealed class Plugin : IDalamudPlugin
{
    private PluginProvider PluginProvider { get; init; }

    public Plugin(IDalamudPluginInterface pluginInterface)
    {
        PluginProvider = new(pluginInterface);

        pluginInterface.UiBuilder.Draw += PluginProvider.Get<WindowSystem>().Draw;
        pluginInterface.UiBuilder.OpenConfigUi += PluginProvider.Get<ConfigWindow>().Toggle;
        pluginInterface.UiBuilder.OpenMainUi += PluginProvider.Get<MainWindow>().Toggle;

        PluginProvider.Init<ICommand>();
        PluginProvider.Init<IModAutoProcessor>();
    }

    public void Dispose() => PluginProvider.Dispose();
}
