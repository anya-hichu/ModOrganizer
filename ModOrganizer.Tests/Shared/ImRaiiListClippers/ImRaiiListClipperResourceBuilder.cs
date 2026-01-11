using Dalamud.Bindings.ImGui;
using Dalamud.Bindings.ImGui.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Dalamuds.Bindings.ImGuis;
using ModOrganizer.Tests.Dalamuds.Bindings.ImGuis.ListClippers;

namespace ModOrganizer.Tests.Shared.ImRaiiListClippers;

public class ImRaiiListClipperResourceBuilder : IBuilder<RaiiGuard<ImRaiiListClipper>>, IStubbableImGui, IStubbableImGuiListClipper
{
    public StubIImGui ImGuiStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIImGuiListClipperPtr ImGuiListClipperPtrStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public int ImGuiListClipperItemsCount { get; set; } = default;
    public float ImGuiListClipperItemsHeight { get; set; } = default;

    public ImRaiiListClipperResourceBuilder WithImRaiiListClipperItemsCount(int count)
    {
        ImGuiListClipperItemsCount = count;

        return this; 
    }

    public ImRaiiListClipperResourceBuilder WithImRaiiListClipperItemsHeight(float height)
    {
        ImGuiListClipperItemsHeight = height;

        return this;
    }

    private ImRaiiListClipper Acquire()
    {
        ImGui.MaybeImplementation = ImGuiStub;

        return new(ImGuiListClipperItemsCount, ImGuiListClipperItemsHeight);
    }

    private static void Release(ImRaiiListClipper raiiListClipper) => raiiListClipper.Dispose();

    public RaiiGuard<ImRaiiListClipper> Build() => new(Acquire, Release);
}
