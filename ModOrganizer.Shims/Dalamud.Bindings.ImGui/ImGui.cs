using System;
using System.Numerics;

namespace Dalamud.Bindings.ImGui;

public class ImGui
{
    public delegate int ImGuiInputTextCallbackDelegate();

    public static IImGui? Instance { get; set; }

    // Dispatch static methods to instance for stubbing
    public static void Text(ImU8String text) => Instance?.Text(text);
    public static bool Button(ImU8String label, Vector2 size) => Instance != null && Instance.Button(label, size);

    public static bool InputText(ImU8String label, scoped ref string buf, int maxLength = ImU8String.AllocFreeBufferSize, ImGuiInputTextFlags flags = ImGuiInputTextFlags.None, ImGuiInputTextCallbackDelegate? callback = null)
    {
        if (Instance == null) return false;

        return Instance.InputText(label, ref buf, maxLength, flags, callback);
    }
}
