using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Containers;
using ModOrganizer.Json.Penumbra.Groups;
using ModOrganizer.Json.Penumbra.Groups.Fakes;
using ModOrganizer.Json.Penumbra.Options;
using ModOrganizer.Json.Readers.Fakes;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Dalamuds.PluginLogs;

namespace ModOrganizer.Tests.Json.Penumbra.Groups.Combinings;

public class GroupCombiningReaderBuilder : IBuilder<GroupCombiningReader>, IStubbablePluginLog
{
    public StubIGroupBaseReader GroupBaseReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIReader<NamedContainer> NamedContainerReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIReader<Option> OptionReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public GroupCombiningReader Build() => new(GroupBaseReaderStub, NamedContainerReaderStub, OptionReaderStub, PluginLogStub);
}
