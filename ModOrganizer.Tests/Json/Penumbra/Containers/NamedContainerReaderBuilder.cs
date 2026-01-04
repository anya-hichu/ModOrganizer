using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Containers;
using ModOrganizer.Json.Readers.Fakes;
using ModOrganizer.Tests.Dalamuds.PluginLogs;

namespace ModOrganizer.Tests.Json.Penumbra.Containers;

public class NamedContainerReaderBuilder : Builder<NamedContainerReader>, IStubbableContainerReader, IStubbablePluginLog
{
    public StubIReader<Container> ContainerReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public override NamedContainerReader Build() => new(ContainerReaderStub, PluginLogStub);
}
