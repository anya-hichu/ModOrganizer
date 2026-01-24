using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;

namespace ModOrganizer.Windows.Managers;

public class WindowManager(IPluginLog pluginLog) : IWindowManager
{
    public WindowSystem? MaybeWindowSystem { get; set; }

    public void Add(Window window)
    {
        if (MaybeWindowSystem == null)
        {
            pluginLog.Error($"Failed to add [{window.GetType().Name}] with name [{window.WindowName}]");
            return;
        }

        MaybeWindowSystem.AddWindow(window);
    }

    public void Remove(Window window)
    {
        if (MaybeWindowSystem == null)
        {
            pluginLog.Error($"Failed to remove [{window.GetType().Name}] with name [{window.WindowName}]");
            return;
        }

        MaybeWindowSystem.RemoveWindow(window);
    } 
}
