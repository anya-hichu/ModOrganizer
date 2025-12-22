using Dalamud.Bindings.ImGui;

namespace ModOrganizer.Shared;

public class ImRaiiListClipper : RaiiGuard<ImGuiListClipperPtr>
{
    public ImRaiiListClipper() : base(ImGui.ImGuiListClipper, clipper => clipper.Destroy())
    {
        // Empty
    }
}
