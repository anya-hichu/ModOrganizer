using Dalamud.Bindings.ImGui.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Shared;

namespace ModOrganizer.Tests.Dalamuds.Bindings.ImGuis;

public class ImGuiStubBuilder : IBuilder<StubIImGui>
{
    public StubIImGui ImGuiStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public ImGuiStubBuilder WithDefaults()
    {
        ImGuiStub.BehaveAsDefaultValue();

        return this;
    }

    public ImGuiStubBuilder WithObserver(IStubObserver observer)
    {
        ImGuiStub.InstanceObserver = observer;

        return this; 
    }

    public StubIImGui Build() => ImGuiStub;
}
