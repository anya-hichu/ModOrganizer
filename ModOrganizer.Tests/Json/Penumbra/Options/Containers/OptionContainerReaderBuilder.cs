using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Containers;
using ModOrganizer.Json.Penumbra.Options;
using ModOrganizer.Json.Penumbra.Options.Containers;
using ModOrganizer.Json.Readers.Fakes;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Json.Penumbra.Containers;

namespace ModOrganizer.Tests.Json.Penumbra.Options.Containers;

public class OptionContainerReaderBuilder : IBuilder<OptionContainerReader>, IStubbableContainerReader, IStubbableOptionReader, IStubbablePluginLog
{
    public StubIReader<Container> ContainerReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIReader<Option> OptionReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public OptionContainerReader Build() => new(ContainerReaderStub, OptionReaderStub, PluginLogStub);
}
