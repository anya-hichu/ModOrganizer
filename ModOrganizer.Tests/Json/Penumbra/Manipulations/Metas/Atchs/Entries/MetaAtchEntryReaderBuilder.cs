using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Atchs.Entries;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Dalamuds.PluginLogs;

namespace ModOrganizer.Tests.Json.Penumbra.Manipulations.Metas.Atchs.Entries;

public class MetaAtchEntryReaderBuilder : IBuilder<MetaAtchEntryReader>, IStubbablePluginLog
{
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public MetaAtchEntryReader Build() => new(PluginLogStub);
}
