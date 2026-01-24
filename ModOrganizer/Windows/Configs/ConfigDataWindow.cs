using Dalamud.Interface.ImGuiFileDialog;
using Dalamud.Interface.Windowing;
using ModOrganizer.Windows.Managers;
using System;

namespace ModOrganizer.Windows.Configs;

public abstract class ConfigDataWindow : Window
{
    private IWindowManager WindowManager { get; init; }
    protected FileDialogManager FileDialogManager { get; init; } = new();

    public ConfigDataWindow(string baseName, IWindowManager windowManager) : base(string.Concat(baseName, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()))
    {
        WindowManager = windowManager;

        IsOpen = true;
        WindowManager.Add(this);
    }

    public override void OnClose() => WindowManager.Remove(this);
}
