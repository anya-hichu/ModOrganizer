using Dalamud.Plugin.Services.Fakes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Groups;
using ModOrganizer.Json.Penumbra.Manipulations;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Asserts.Fakes;
using ModOrganizer.Json.Readers.Elements.Fakes;
using ModOrganizer.Json.Readers.Fakes;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Json.Readers;
using ModOrganizer.Tests.Json.Readers.Asserts;

namespace ModOrganizer.Tests.Json.Penumbra.Groups;

public class GroupGenericReaderBuilder : IBuilder<GroupGenericReader>, IStubbableAssert, IStubbablePluginLog, IStubbableReaderProvider<Group>
{
    public StubIAssert AssertStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

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

    public GroupGenericReader Build() => new(AssertStub, ElementReaderStub, GroupCombiningReaderStub, GroupImcReaderStub, GroupMultiReaderStub, GroupSingleReaderStub, PluginLogStub);
}
