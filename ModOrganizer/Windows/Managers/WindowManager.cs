using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;

namespace ModOrganizer.Windows.Managers;

public class WindowManager(IPluginLog pluginLog) : IWindowManager
{
    public WindowSystem? MaybeWindowSystem { get; set; }

    public void Add(Window window)
    {
        if (MaybeWindowSystem == null)
        {
            pluginLog.Error($"Failed to add [{window.GetType().Name}] because window system is not defined");
            return;
        }

        MaybeWindowSystem.AddWindow(window);
    }

    public void Remove(Window window) => MaybeWindowSystem?.RemoveWindow(window);
}
