using Dalamud.Bindings.ImGui;
using Dalamud.Bindings.ImGui.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Dalamuds.Bindings.ImGuis;
using ModOrganizer.Windows;
using ModOrganizer.Windows.Results.Rules.Fakes;

namespace ModOrganizer.Tests.Windows.Previews;

public class PreviewWindowResourceBuilder : IBuilder<RaiiGuard<PreviewWindow>>, IStubbableImGui
{
    public StubIImGui ImGuiStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIRuleResultFileSystem RuleResultFileSystemStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    private PreviewWindow Acquire()
    {
        ImGui.MaybeImplementation = ImGuiStub;

        return new(RuleResultFileSystemStub);
    }

    private static void Release(PreviewWindow _) => ImGui.MaybeImplementation = null;

    public RaiiGuard<PreviewWindow> Build() => new(Acquire, Release);
}
