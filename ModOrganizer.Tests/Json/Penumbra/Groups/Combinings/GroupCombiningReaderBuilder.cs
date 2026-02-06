using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Containers;
using ModOrganizer.Json.Penumbra.Groups.Bases.Fakes;
using ModOrganizer.Json.Penumbra.Groups.Combinings;
using ModOrganizer.Json.Penumbra.Options;
using ModOrganizer.Json.Readers.Fakes;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Json.Penumbra.Containers;
using ModOrganizer.Tests.Json.Penumbra.Groups.Bases;
using ModOrganizer.Tests.Json.Penumbra.Options;

namespace ModOrganizer.Tests.Json.Penumbra.Groups.Combinings;

public class GroupCombiningReaderBuilder : IBuilder<GroupCombiningReader>, IStubbablePluginLog, IStubbableGroupBaseReader, IStubbableNamedContainerReader, IStubbableOptionReader
{
    public StubIGroupBaseReader GroupBaseReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIReader<NamedContainer> NamedContainerReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIReader<Option> OptionReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public GroupCombiningReader Build() => new(GroupBaseReaderStub, NamedContainerReaderStub, OptionReaderStub, PluginLogStub);
}
