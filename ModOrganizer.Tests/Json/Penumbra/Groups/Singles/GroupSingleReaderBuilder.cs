using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Groups.Bases.Fakes;
using ModOrganizer.Json.Penumbra.Groups.Singles;
using ModOrganizer.Json.Penumbra.Options.Containers;
using ModOrganizer.Json.Readers.Fakes;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Json.Penumbra.Groups.Bases;
using ModOrganizer.Tests.Json.Penumbra.Options.Containers;

namespace ModOrganizer.Tests.Json.Penumbra.Groups.Singles;

public class GroupSingleReaderBuilder : IBuilder<GroupSingleReader>, IStubbableGroupBaseReader, IStubbableOptionContainerReader, IStubbablePluginLog
{
    public StubIGroupBaseReader GroupBaseReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIReader<OptionContainer> OptionContainerStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public GroupSingleReader Build() => new(GroupBaseReaderStub, OptionContainerStub, PluginLogStub);
}
