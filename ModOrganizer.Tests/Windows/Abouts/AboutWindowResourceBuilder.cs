using Dalamud.Bindings.ImGui;
using Dalamud.Bindings.ImGui.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Dalamuds.Bindings.ImGuis;
using ModOrganizer.Windows;

namespace ModOrganizer.Tests.Windows.Abouts;

public class AboutWindowResourceBuilder : IBuilder<RaiiGuard<AboutWindow>>, IStubbableImGui
{
    public StubIImGui ImGuiStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    private AboutWindow Acquire()
    {
        ImGui.MaybeImplementation = ImGuiStub;

        return new();
    }

    private static void Release(AboutWindow _) => ImGui.MaybeImplementation = null;

    public RaiiGuard<AboutWindow> Build() => new(Acquire, Release);
}
