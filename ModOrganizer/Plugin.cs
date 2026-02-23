using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using ModOrganizer.Commands;
using ModOrganizer.Mods;
using ModOrganizer.Providers;
using ModOrganizer.Windows;
using ModOrganizer.Windows.Configs;

namespace ModOrganizer;

public sealed class Plugin : IDalamudPlugin
{
    private PluginProvider PluginProvider { get; init; }

    public Plugin(IDalamudPluginInterface pluginInterface)
    {
        PluginProvider = new(pluginInterface);

        PluginProvider.Init<ICommand>();
        PluginProvider.Init<IModAutoProcessor>();

        var uiBuilder = pluginInterface.UiBuilder;

        uiBuilder.Draw += PluginProvider.Get<WindowSystem>().Draw;
        uiBuilder.OpenConfigUi += PluginProvider.Get<ConfigWindow>().Toggle;
        uiBuilder.OpenMainUi += PluginProvider.Get<MainWindow>().Toggle;
    }

    public void Dispose() => PluginProvider.Dispose();
}
