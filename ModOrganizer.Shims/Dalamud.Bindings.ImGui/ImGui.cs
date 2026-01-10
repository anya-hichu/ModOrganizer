using System.Numerics;

namespace Dalamud.Bindings.ImGui;

public class ImGui
{
    public static IImGui? Instance { get; }

    // Dispatch to instance to allow stubbing
    public static void Text(ImU8String text) => Instance?.Text(text);
    public static bool Button(ImU8String label, Vector2 size) => Instance != null && Instance.Button(label, size);
}
