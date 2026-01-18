using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Manipulations;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Atchs;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Atrs;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Eqdps;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Eqps;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Ests;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Geqps;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Gmps;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Imcs;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Rsps;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Shps;
using ModOrganizer.Json.Readers.Fakes;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Json.Readers;

namespace ModOrganizer.Tests.Json.Penumbra.Manipulations;

public class ManipulationWrapperGenericReaderBuilder : IBuilder<ManipulationWrapperGenericReader>, IStubbablePluginLog, IStubbableReaderProvider<ManipulationWrapper>
{
    public StubIReader<ManipulationWrapper> MetaAtchWrapperReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIReader<ManipulationWrapper> MetaAtrWrapperReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIReader<ManipulationWrapper> MetaEqdpWrapperReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIReader<ManipulationWrapper> MetaEqpWrapperReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIReader<ManipulationWrapper> MetaEstWrapperReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIReader<ManipulationWrapper> MetaGeqpWrapperReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIReader<ManipulationWrapper> MetaGmpWrapperReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIReader<ManipulationWrapper> MetaImcWrapperReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIReader<ManipulationWrapper> MetaRspWrapperReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIReader<ManipulationWrapper> MetaShpWrapperReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public StubIReader<ManipulationWrapper> GetReaderStub(string type) => type switch
    {
        _ when type == MetaAtchWrapperReader.TYPE => MetaAtchWrapperReaderStub,
        _ when type == MetaAtrWrapperReader.TYPE => MetaAtrWrapperReaderStub,
        _ when type == MetaEqdpWrapperReader.TYPE => MetaEqdpWrapperReaderStub,
        _ when type == MetaEqpWrapperReader.TYPE => MetaEqpWrapperReaderStub,
        _ when type == MetaEstWrapperReader.TYPE => MetaEstWrapperReaderStub,
        _ when type == MetaGeqpWrapperReader.TYPE => MetaGeqpWrapperReaderStub,
        _ when type == MetaGmpWrapperReader.TYPE => MetaGmpWrapperReaderStub,
        _ when type == MetaImcWrapperReader.TYPE => MetaImcWrapperReaderStub,
        _ when type == MetaRspWrapperReader.TYPE => MetaRspWrapperReaderStub,
        _ when type == MetaShpWrapperReader.TYPE => MetaShpWrapperReaderStub,

        _ => throw new NotImplementedException()
    };

    public ManipulationWrapperGenericReader Build() => new(MetaAtchWrapperReaderStub, MetaAtrWrapperReaderStub, MetaEqdpWrapperReaderStub, MetaEqpWrapperReaderStub, MetaEstWrapperReaderStub,
        MetaGeqpWrapperReaderStub, MetaGmpWrapperReaderStub, MetaImcWrapperReaderStub, MetaRspWrapperReaderStub, MetaShpWrapperReaderStub, PluginLogStub);
}
