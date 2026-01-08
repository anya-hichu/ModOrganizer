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
    private RootProvider RootProvider { get; init; }

    public Plugin(IDalamudPluginInterface pluginInterface)
    {
        RootProvider = pluginInterface.Create<RootProvider>()!;

        RootProvider.Init<ICommand>();
        RootProvider.Init<IModAutoProcessor>();

        pluginInterface.UiBuilder.Draw += RootProvider.Get<WindowSystem>().Draw;
        pluginInterface.UiBuilder.OpenConfigUi += RootProvider.Get<ConfigWindow>().Toggle;
        pluginInterface.UiBuilder.OpenMainUi += RootProvider.Get<MainWindow>().Toggle;
    }

    public void Dispose() => RootProvider.Dispose();
}
