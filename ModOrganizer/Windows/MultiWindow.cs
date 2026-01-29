using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using ModOrganizer.Windows.Managers;
using System;

namespace ModOrganizer.Windows;

public abstract class MultiWindow : Window
{
    private IWindowManager WindowManager { get; init; }

    public MultiWindow(string label, IWindowManager windowManager) : base($"{label}###multiWindow{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}")
    {
        WindowManager = windowManager;

        IsOpen = true;
        WindowManager.Add(this);
    }

    public override void OnClose() => WindowManager.Remove(this);
}
