using Dalamud.Plugin.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Providers;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Dalamuds.PluginInterfaces;

namespace ModOrganizer.Tests.Providers;

public class PluginProviderBuilder : IBuilder<PluginProvider>, IStubbablePluginInterface
{
    public StubIDalamudPluginInterface PluginInterfaceStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public PluginProvider Build() => new(PluginInterfaceStub);
}
