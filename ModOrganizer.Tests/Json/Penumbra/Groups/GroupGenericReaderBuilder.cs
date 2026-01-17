using Dalamud.Plugin.Services.Fakes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Groups;
using ModOrganizer.Json.Penumbra.Manipulations;
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
    private static readonly HashSet<string> READER_TYPES = [GroupCombiningReader.TYPE, GroupImcReader.TYPE, GroupMultiReader.TYPE, GroupSingleReader.TYPE];

    public StubIAssert AssertStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public StubIElementReader ElementReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    private ServiceProvider ReaderStubProvider { get; init; }

    public GroupGenericReaderBuilder()
    {
        var collection = new ServiceCollection();

        foreach (var type in READER_TYPES) collection.AddKeyedSingleton<StubIReader<Group>>(type, (_, __) => new() { InstanceBehavior = StubBehaviors.NotImplemented });

        ReaderStubProvider = collection.BuildServiceProvider();
    }

    public StubIReader<Group> GetReaderStub(string type) => ReaderStubProvider.GetRequiredKeyedService<StubIReader<Group>>(type);

    public GroupGenericReader Build() => new(
        AssertStub,
        GetReaderStub(GroupCombiningReader.TYPE),
        GetReaderStub(GroupImcReader.TYPE),
        GetReaderStub(GroupMultiReader.TYPE),
        GetReaderStub(GroupSingleReader.TYPE), 
        ElementReaderStub, 
        PluginLogStub
    );
}
