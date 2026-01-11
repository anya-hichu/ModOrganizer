using System;
using System.Numerics;

namespace Dalamud.Bindings.ImGui;

public static class ImGui
{
    public delegate int ImGuiInputTextCallbackDelegate();

    public static IImGui? MaybeImplementation { get; set; }

    public static bool Button(ImU8String label, Vector2 size) => GetImplemention().Button(label, size);

    public static bool Checkbox(ImU8String label, ref bool value) => GetImplemention().Checkbox(label, ref value);

    public static uint GetColorU32(ImGuiCol idx) => GetImplemention().GetColorU32(idx);
    public static float GetWindowWidth() => GetImplemention().GetWindowWidth();
    
    public static bool InputText(ImU8String label, scoped ref string buf, int maxLength = ImU8String.AllocFreeBufferSize, ImGuiInputTextFlags flags = ImGuiInputTextFlags.None, ImGuiInputTextCallbackDelegate? callback = null) =>
        GetImplemention().InputText(label, ref buf, maxLength, flags, callback);
    public static bool InputTextWithHint(ImU8String label, ImU8String hint, scoped ref string buf, int maxLength = ImU8String.AllocFreeBufferSize, ImGuiInputTextFlags flags = ImGuiInputTextFlags.None, ImGuiInputTextCallbackDelegate? callback = null) =>
        GetImplemention().InputTextWithHint(label, hint, ref buf, maxLength, flags, callback);
    
    public static ImGuiListClipperPtr ImGuiListClipper() => GetImplemention().ImGuiListClipper();

    public static bool IsItemClicked() => GetImplemention().IsItemClicked();
    public static bool IsItemHovered() => GetImplemention().IsItemHovered();

    public static void PopStyleColor(int count) => GetImplemention().PopStyleColor(count);
    public static void PushStyleColor(ImGuiCol idx, Vector4 col) => GetImplemention().PushStyleColor(idx, col);

    public static void SameLine() => GetImplemention().SameLine();
    public static void SameLine(float offsetFromStartX) => GetImplemention().SameLine(offsetFromStartX);

    public static void SetTooltip(ImU8String text) => GetImplemention().SetTooltip(text);

    public static void Text(ImU8String text) => GetImplemention().Text(text);

    public static bool TreeNodeEx(ImU8String id, ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.None, ImU8String label = default) => GetImplemention().TreeNodeEx(id, flags, label);
    public static void TreePop() => GetImplemention().TreePop();

    private static IImGui GetImplemention() => MaybeImplementation == null ? throw new NotImplementedException() : MaybeImplementation;
}
