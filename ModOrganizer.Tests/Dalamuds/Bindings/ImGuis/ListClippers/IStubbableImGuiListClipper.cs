using Dalamud.Bindings.ImGui.Fakes;

namespace ModOrganizer.Tests.Dalamuds.Bindings.ImGuis.ListClippers;

public interface IStubbableImGuiListClipper
{
    StubIImGuiListClipperPtr ImGuiListClipperStub { get; }
    int ImGuiListClipperItemsCount { set; }
    float ImGuiListClipperItemsHeight { set; }
}
