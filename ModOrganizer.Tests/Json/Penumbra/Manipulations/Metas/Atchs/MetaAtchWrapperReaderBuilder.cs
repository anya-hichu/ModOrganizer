using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Atchs;
using ModOrganizer.Json.Readers.Fakes;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Dalamuds.PluginLogs;

namespace ModOrganizer.Tests.Json.Penumbra.Manipulations.Metas.Atchs;

public class MetaAtchWrapperReaderBuilder : IBuilder<MetaAtchWrapperReader>, IStubbableMetaAtchReader, IStubbablePluginLog
{
    public StubIReader<MetaAtch> MetaAtchReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public MetaAtchWrapperReader Build() => new(MetaAtchReaderStub, PluginLogStub);
}
