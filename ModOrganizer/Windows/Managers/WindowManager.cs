using Dalamud.Interface.Windowing;
using System;

namespace ModOrganizer.Windows.Managers;

public class WindowManager() : IWindowManager
{
    public WindowSystem? MaybeWindowSystem { get; set; }

    public void Add(Window window) => GetWindowSystem().AddWindow(window);

    public void Remove(Window window) => GetWindowSystem().RemoveWindow(window);

    private WindowSystem GetWindowSystem() => MaybeWindowSystem ?? throw new NotImplementedException();
}
