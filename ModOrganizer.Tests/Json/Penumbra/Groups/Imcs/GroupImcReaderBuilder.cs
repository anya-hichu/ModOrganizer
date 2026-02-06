using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Groups.Bases.Fakes;
using ModOrganizer.Json.Penumbra.Groups.Imcs;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Imcs.Entries;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Imcs.Identifiers;
using ModOrganizer.Json.Penumbra.Options.Imcs.Fakes;
using ModOrganizer.Json.Readers.Fakes;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Json.Penumbra.Groups.Bases;
using ModOrganizer.Tests.Json.Penumbra.Manipulations.Metas.Imcs;
using ModOrganizer.Tests.Json.Penumbra.Options.Imcs;

namespace ModOrganizer.Tests.Json.Penumbra.Groups.Imcs;

public class GroupImcReaderBuilder : IBuilder<GroupImcReader>, IStubbableGroupBaseReader, IStubbableMetaImcEntryReader, IStubbableMetaImcIdentifierReader, IStubbableOptionImcGenericReader, IStubbablePluginLog
{
    public StubIGroupBaseReader GroupBaseReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIReader<MetaImcEntry> MetaImcEntryReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIReader<MetaImcIdentifier> MetaImcIdentifierReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIOptionImcGenericReader OptionImcGenericReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public GroupImcReader Build() => new(GroupBaseReaderStub, MetaImcEntryReaderStub, MetaImcIdentifierReaderStub, OptionImcGenericReaderStub, PluginLogStub);
}
