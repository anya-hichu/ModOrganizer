using System;
using System.Numerics;

namespace Dalamud.Bindings.ImGui;

public static class ImGui
{
    public delegate int ImGuiInputTextCallbackDelegate();

    public static IImGui? MaybeImplementation { get; set; }

    public static bool Button(ImU8String label, Vector2 size) => GetImplementation().Button(label, size);

    public static bool Checkbox(ImU8String label, ref bool value) => GetImplementation().Checkbox(label, ref value);

    public static uint GetColorU32(ImGuiCol idx) => GetImplementation().GetColorU32(idx);
    public static float GetWindowWidth() => GetImplementation().GetWindowWidth();
    
    public static bool InputText(ImU8String label, scoped ref string buf, int maxLength = ImU8String.AllocFreeBufferSize, ImGuiInputTextFlags flags = ImGuiInputTextFlags.None, ImGuiInputTextCallbackDelegate? callback = null) =>
        GetImplementation().InputText(label, ref buf, maxLength, flags, callback);
    public static bool InputTextWithHint(ImU8String label, ImU8String hint, scoped ref string buf, int maxLength = ImU8String.AllocFreeBufferSize, ImGuiInputTextFlags flags = ImGuiInputTextFlags.None, ImGuiInputTextCallbackDelegate? callback = null) =>
        GetImplementation().InputTextWithHint(label, hint, ref buf, maxLength, flags, callback);
    
    public static ImGuiListClipperPtr ImGuiListClipper() => GetImplementation().ImGuiListClipper();

    public static bool IsItemClicked() => GetImplementation().IsItemClicked();
    public static bool IsItemHovered() => GetImplementation().IsItemHovered();

    public static void PopStyleColor(int count) => GetImplementation().PopStyleColor(count);
    public static void PushStyleColor(ImGuiCol idx, Vector4 col) => GetImplementation().PushStyleColor(idx, col);

    public static void SameLine() => GetImplementation().SameLine();
    public static void SameLine(float offsetFromStartX) => GetImplementation().SameLine(offsetFromStartX);

    public static void SetTooltip(ImU8String text) => GetImplementation().SetTooltip(text);

    public static void Text(ImU8String text) => GetImplementation().Text(text);

    public static bool TreeNodeEx(ImU8String id, ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.None, ImU8String label = default) => 
        GetImplementation().TreeNodeEx(id, flags, label);

    public static void TreePop() => GetImplementation().TreePop();

    private static IImGui GetImplementation() => MaybeImplementation ?? throw new NotImplementedException();
}
