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

    private int ItemsCount { get; set; } = default;
    private float ItemsHeight { get; set; } = default;

    public ImRaiiListClipperResourceBuilder() 
        => ImGuiStub.ImGuiListClipper = () => new() { MaybeImplementation = ImGuiListClipperStub };

    public ImRaiiListClipperResourceBuilder WithItemsCount(int count)
    {
        ItemsCount = count;

        return this; 
    }

    public ImRaiiListClipperResourceBuilder WithItemsHeight(float height)
    {
        ItemsHeight = height;

        return this;
    }

    private ImRaiiListClipper Acquire()
    {
        ImGui.MaybeImplementation = ImGuiStub;

        return new(ItemsCount, ItemsHeight);
    }

    private static void Release(ImRaiiListClipper imRaiiListClipper) 
    {
        imRaiiListClipper.Dispose();

        ImGui.MaybeImplementation = null;
    } 

    public RaiiGuard<ImRaiiListClipper> Build() => new(Acquire, Release);
}
