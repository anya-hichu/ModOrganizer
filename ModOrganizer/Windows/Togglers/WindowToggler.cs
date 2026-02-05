using Dalamud.Interface.Windowing;
using System;
using System.Linq;

namespace ModOrganizer.Windows.Togglers;

public class WindowToggler : IWindowToggler
{
    public WindowSystem? MaybeWindowSystem { get; set; }

    public void Toggle<T>() where T : Window
    {
        var windows = GetWindowSystem().Windows.OfType<T>();
        foreach (var window in windows) window.Toggle();
    }

    private WindowSystem GetWindowSystem() => MaybeWindowSystem ?? throw new NotImplementedException();
}
