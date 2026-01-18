using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Containers;
using ModOrganizer.Json.Penumbra.DefaultMods;
using ModOrganizer.Json.Readers.Elements.Fakes;
using ModOrganizer.Json.Readers.Fakes;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Json.Penumbra.Containers;
using ModOrganizer.Tests.Json.Readers.Elements;

namespace ModOrganizer.Tests.Json.Penumbra.DefaultMods;

public class DefaultModReaderBuilder : IBuilder<DefaultModReader>, IStubbableContainerReader, IStubbableElementReader, IStubbablePluginLog
{
    public StubIReader<Container> ContainerReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIElementReader ElementReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public DefaultModReader Build() => new(ContainerReaderStub, ElementReaderStub, PluginLogStub);
}
