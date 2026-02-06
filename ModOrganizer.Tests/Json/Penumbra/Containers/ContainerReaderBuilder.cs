using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Containers;
using ModOrganizer.Json.Penumbra.Manipulations.Wrappers.Generics.Fakes;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Json.Penumbra.Manipulations.Wrappers.Generics;

namespace ModOrganizer.Tests.Json.Penumbra.Containers;

public class ContainerReaderBuilder : IBuilder<ContainerReader>, IStubbableManipulationWrapperGenericReader, IStubbablePluginLog
{
    public StubIManipulationWrapperGenericReader ManipulationWrapperGenericReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public ContainerReader Build() => new(ManipulationWrapperGenericReaderStub, PluginLogStub);
}
