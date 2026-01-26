using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using ModOrganizer.Windows.Managers;
using System;

namespace ModOrganizer.Windows;

public abstract class MultiWindow : Window
{
    protected long SuffixId { get; init; }
    private IWindowManager WindowManager { get; init; }

    public MultiWindow(string baseName, long suffixId, IWindowManager windowManager) : base(string.Concat(baseName, suffixId))
    {
        SuffixId = suffixId;
        WindowManager = windowManager;

        IsOpen = true;
        WindowManager.Add(this);
    }

    protected static long GenerateMonotonicId() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    public override void OnClose() => WindowManager.Remove(this);
}
