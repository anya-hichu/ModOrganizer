using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Groups;
using ModOrganizer.Json.Readers.Asserts.Fakes;
using ModOrganizer.Json.Readers.Elements.Fakes;
using ModOrganizer.Json.Readers.Fakes;
using ModOrganizer.Shared;

namespace ModOrganizer.Tests.Json.Penumbra.Groups;

public class GroupReaderFactoryBuilder : IBuilder<GroupGenericReader>
{
    public StubIAssert AssertStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public StubIReader<Group> GroupCombiningReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIReader<Group> GroupImcReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIReader<Group> GroupMultiReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIReader<Group> GroupSingleReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public StubIElementReader ElementReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public GroupGenericReader Build() => new(AssertStub, GroupCombiningReaderStub, GroupImcReaderStub, GroupMultiReaderStub, GroupSingleReaderStub, ElementReaderStub, PluginLogStub);
}
