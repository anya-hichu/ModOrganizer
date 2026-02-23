using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Atchs;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Atchs.Entries;
using ModOrganizer.Json.Readers.Fakes;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Json.Penumbra.Manipulations.Metas.Atchs.Entries;

namespace ModOrganizer.Tests.Json.Penumbra.Manipulations.Metas.Atchs;

internal class MetaAtchReaderBuilder : IBuilder<MetaAtchReader>, IStubbableMetaAtchEntryReader, IStubbablePluginLog
{
    public StubIReader<MetaAtchEntry> MetaAtchEntryReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public MetaAtchReader Build() => new(MetaAtchEntryReaderStub, PluginLogStub);
}
