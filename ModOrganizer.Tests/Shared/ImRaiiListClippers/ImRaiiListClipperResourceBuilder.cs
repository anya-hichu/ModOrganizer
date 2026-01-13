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
    public StubIImGuiListClipperPtr ImGuiListClipperStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    private int ImRaiiListClipperItemsCount { get; set; } = default;
    private float ImRaiiListClipperItemsHeight { get; set; } = default;

    public ImRaiiListClipperResourceBuilder() 
        => ImGuiStub.ImGuiListClipper = () => new() { MaybeImplementation = ImGuiListClipperStub };

    public ImRaiiListClipperResourceBuilder WithImRaiiListClipperItemsCount(int count)
    {
        ImRaiiListClipperItemsCount = count;

        return this; 
    }

    public ImRaiiListClipperResourceBuilder WithImRaiiListClipperItemsHeight(float height)
    {
        ImRaiiListClipperItemsHeight = height;

        return this;
    }

    private ImRaiiListClipper Acquire()
    {
        ImGui.MaybeImplementation = ImGuiStub;

        return new(ImRaiiListClipperItemsCount, ImRaiiListClipperItemsHeight);
    }

    private static void Release(ImRaiiListClipper imRaiiListClipper) 
    {
        imRaiiListClipper.Dispose();

        ImGui.MaybeImplementation = null;
    } 

    public RaiiGuard<ImRaiiListClipper> Build() => new(Acquire, Release);
}
