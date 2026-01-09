using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Containers;
using ModOrganizer.Json.Penumbra.Manipulations.Fakes;
using ModOrganizer.Json.Readers.Asserts.Fakes;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Json.Penumbra.Manipulations;
using ModOrganizer.Tests.Json.Readers.Asserts;

namespace ModOrganizer.Tests.Json.Penumbra.Containers;

public class ContainerReaderBuilder : IBuilder<ContainerReader>, IStubbableAssert, IStubbableManipulationWrapperGenericReader, IStubbablePluginLog
{
    public StubIAssert AssertStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIManipulationWrapperGenericReader ManipulationWrapperGenericReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public ContainerReader Build() => new(AssertStub, ManipulationWrapperGenericReaderStub, PluginLogStub);
}
