using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ModOrganizer.Commands;
using ModOrganizer.Mods;
using ModOrganizer.Providers;
using ModOrganizer.Windows;
using ModOrganizer.Windows.Configs;

namespace ModOrganizer;

public sealed class Plugin : IDalamudPlugin, IProvider
{
    private RootProvider RootProvider { get; init; }

    public Plugin(IDalamudPluginInterface pluginInterface)
    {
        RootProvider = pluginInterface.Create<RootProvider>()!;

        Init<ICommand>();
        Init<IModAutoProcessor>();

        pluginInterface.UiBuilder.Draw += Get<WindowSystem>().Draw;
        pluginInterface.UiBuilder.OpenConfigUi += Get<ConfigWindow>().Toggle;
        pluginInterface.UiBuilder.OpenMainUi += Get<MainWindow>().Toggle;
    }

    public void Dispose() => RootProvider.Dispose();

    public T Get<T>() where T : notnull => RootProvider.Get<T>();
    private void Init<T>() where T : notnull => Get<T>();
}
