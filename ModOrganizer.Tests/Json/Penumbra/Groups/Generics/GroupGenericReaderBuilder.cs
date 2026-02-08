using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Groups;
using ModOrganizer.Json.Penumbra.Groups.Combinings;
using ModOrganizer.Json.Penumbra.Groups.Generics;
using ModOrganizer.Json.Penumbra.Groups.Imcs;
using ModOrganizer.Json.Penumbra.Groups.Multis;
using ModOrganizer.Json.Penumbra.Groups.Singles;
using ModOrganizer.Json.Readers.Elements.Fakes;
using ModOrganizer.Json.Readers.Fakes;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Json.Readers;

namespace ModOrganizer.Tests.Json.Penumbra.Groups;

public class GroupGenericReaderBuilder : IBuilder<GroupGenericReader>, IStubbablePluginLog, IStubbableReaderProvider<Group>
{
    public StubIElementReader ElementReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIReader<Group> GroupCombiningReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIReader<Group> GroupImcReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIReader<Group> GroupMultiReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIReader<Group> GroupSingleReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public StubIReader<Group> GetReaderStub(string type) => type switch
    {
        _ when type == GroupCombiningReader.TYPE => GroupCombiningReaderStub,
        _ when type == GroupImcReader.TYPE => GroupImcReaderStub,
        _ when type == GroupMultiReader.TYPE => GroupMultiReaderStub,
        _ when type == GroupSingleReader.TYPE => GroupSingleReaderStub,

        _ => throw new NotImplementedException()
    };

    public GroupGenericReader Build() => new(ElementReaderStub, GroupCombiningReaderStub, GroupImcReaderStub, GroupMultiReaderStub, GroupSingleReaderStub, PluginLogStub);
}
