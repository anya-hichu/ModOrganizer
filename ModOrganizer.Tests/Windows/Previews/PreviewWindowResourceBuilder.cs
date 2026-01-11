using Dalamud.Bindings.ImGui;
using Dalamud.Bindings.ImGui.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Dalamuds.Bindings.ImGuis;
using ModOrganizer.Virtuals;
using ModOrganizer.Windows;
using ModOrganizer.Windows.Results.Rules;
using ModOrganizer.Windows.Results.Rules.Fakes;

namespace ModOrganizer.Tests.Windows.Previews;

public class PreviewWindowResourceBuilder : IBuilder<RaiiGuard<PreviewWindow>>, IStubbableImGui
{
    public StubIImGui ImGuiStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIRuleResultFileSystem RuleResultFileSystemStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public PreviewWindowResourceBuilder WithRuleResultFileSystemRootFolder(VirtualFolder stubValue)
    {
        RuleResultFileSystemStub.GetRootFolder = () => stubValue;

        return this;
    }

    public PreviewWindowResourceBuilder WithRuleResultFileSystemTryGetFileData(RulePathResult? stubValue)
    {
        RuleResultFileSystemStub.TryGetFileDataVirtualFileRulePathResultOut = (_, out fileData) =>
        {
            fileData = stubValue;

            return stubValue != null;
        };

        return this;
    }

    private PreviewWindow Acquire()
    {
        ImGui.MaybeImplementation = ImGuiStub;

        return new(RuleResultFileSystemStub);
    }

    private static void Release(PreviewWindow _) => ImGui.MaybeImplementation = null;

    public RaiiGuard<PreviewWindow> Build() => new(Acquire, Release);
}
