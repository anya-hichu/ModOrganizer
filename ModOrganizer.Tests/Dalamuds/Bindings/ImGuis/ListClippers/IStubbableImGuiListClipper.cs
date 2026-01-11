using Dalamud.Bindings.ImGui.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;

namespace ModOrganizer.Tests.Dalamuds.Bindings.ImGuis.ListClippers;

public interface IStubbableImGuiListClipper : IStubbableImGui
{
    StubIImGuiListClipperPtr ImGuiListClipperPtrStub { get; }
}
