using System.Numerics;
using static Dalamud.Bindings.ImGui.ImGui;

namespace Dalamud.Bindings.ImGui;

public interface IImGui
{
    void Text(ImU8String text);
    bool Button(ImU8String label, Vector2 size);
    bool InputText(ImU8String label, scoped ref string buf, int maxLength = 0, ImGuiInputTextFlags flags = ImGuiInputTextFlags.None, ImGuiInputTextCallbackDelegate? callback = null);
}
