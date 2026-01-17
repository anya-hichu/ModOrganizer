using Dalamud.Bindings.ImGui.Fakes;

namespace ModOrganizer.Tests.Dalamuds.Bindings.ImGuis;

public interface IStubbableImGui
{
    StubIImGui ImGuiStub { get; }
}
