using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using System.Linq;

namespace ModOrganizer.Windows.Togglers;

public class WindowToggler(IPluginLog pluginLog) : IWindowToggler
{
    public WindowSystem? MaybeWindowSystem { get; set; }

    public void Toggle<T>() where T : Window
    {
        if (MaybeWindowSystem == null)
        {
            pluginLog.Error($"Failed to toggle [{typeof(T).Name}]");
            return;
        }

        foreach (var window in MaybeWindowSystem.Windows.OfType<T>()) window.Toggle();
    }
}
