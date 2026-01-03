using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Containers;
using ModOrganizer.Json.Penumbra.Manipulations;
using ModOrganizer.Json.Readers.Fakes;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Json.Penumbra.Manipulations;

namespace ModOrganizer.Tests.Json.Penumbra.Containers;

public class ContainerReaderBuilder : Builder<ContainerReader>, IStubbableManipulationWrapperReader, IStubbablePluginLog
{
    public StubIReader<ManipulationWrapper> ManipulationWrapperReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public override ContainerReader Build() => new(ManipulationWrapperReaderStub, PluginLogStub);
}
