using System.Numerics;

namespace Dalamud.Bindings.ImGui;

public interface IImGui
{
    bool Button(ImU8String label, Vector2 size);
    bool Checkbox(ImU8String label, ref bool value);

    uint GetColorU32(ImGuiCol idx);
    float GetWindowWidth();

    bool InputText(ImU8String label, scoped ref string buf, int maxLength = 0, ImGuiInputTextFlags flags = ImGuiInputTextFlags.None, ImGui.ImGuiInputTextCallbackDelegate? callback = null);
    bool InputTextWithHint(ImU8String label, ImU8String hint, scoped ref string buf, int maxLength = ImU8String.AllocFreeBufferSize, ImGuiInputTextFlags flags = ImGuiInputTextFlags.None, ImGui.ImGuiInputTextCallbackDelegate? callback = null);

    ImGuiListClipperPtr ImGuiListClipper();

    bool IsItemClicked();
    bool IsItemHovered();

    void PopStyleColor(int count);
    void PushStyleColor(ImGuiCol idx, Vector4 col);

    void SameLine();
    void SameLine(float offsetFromStartX);

    void SetTooltip(ImU8String text);

    void Text(ImU8String text);

    bool TreeNodeEx(ImU8String id, ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.None, ImU8String label = default);
    void TreePop();
}
