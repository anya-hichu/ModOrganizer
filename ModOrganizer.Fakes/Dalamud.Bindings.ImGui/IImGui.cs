using System.Numerics;

namespace Dalamud.Bindings.ImGui;

public interface IImGui
{
    void Text(ImU8String text);
    bool Button(ImU8String label, Vector2 size);
}
