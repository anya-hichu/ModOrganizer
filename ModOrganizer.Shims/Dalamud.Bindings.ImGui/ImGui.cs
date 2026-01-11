using System;
using System.Numerics;
using System.Reflection.Emit;

namespace Dalamud.Bindings.ImGui;

public class ImGui
{
    public static IImGui? MaybeImplementation { get; set; }

    public static void Text(ImU8String text) => MaybeImplementation!.Text(text);
    public static bool Button(ImU8String label, Vector2 size) => MaybeImplementation!.Button(label, size);

    public static bool InputText(ImU8String label, scoped ref string buf, int maxLength = ImU8String.AllocFreeBufferSize, ImGuiInputTextFlags flags = ImGuiInputTextFlags.None, ImGuiInputTextCallbackDelegate? callback = null) => 
        MaybeImplementation!.InputText(label, ref buf, maxLength, flags, callback);

    public static ImGuiListClipperPtr ImGuiListClipper() => MaybeImplementation!.ImGuiListClipper();
}
