using Dalamud.Plugin.Services.Fakes;
using Microsoft.Extensions.DependencyInjection;
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
using ModOrganizer.Json.Readers.Asserts.Fakes;
using ModOrganizer.Json.Readers.Fakes;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Json.Readers;
using ModOrganizer.Tests.Json.Readers.Asserts;

namespace ModOrganizer.Tests.Json.Penumbra.Manipulations;

public class ManipulationWrapperGenericReaderBuilder : IBuilder<ManipulationWrapperGenericReader>, IStubbableAssert, IStubbablePluginLog, IStubbableReaderProvider<ManipulationWrapper>
{
    private static readonly HashSet<string> READER_TYPES = [MetaAtchWrapperReader.TYPE, MetaAtrWrapperReader.TYPE, MetaEqdpWrapperReader.TYPE, MetaEqpWrapperReader.TYPE,
            MetaEstWrapperReader.TYPE, MetaGeqpWrapperReader.TYPE, MetaGmpWrapperReader.TYPE, MetaImcWrapperReader.TYPE, MetaRspWrapperReader.TYPE, MetaShpWrapperReader.TYPE];

    public StubIAssert AssertStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    private ServiceProvider ReaderStubProvider { get; init; }

    public ManipulationWrapperGenericReaderBuilder()
    {
        var collection = new ServiceCollection();

        foreach (var type in READER_TYPES) collection.AddKeyedSingleton<StubIReader<ManipulationWrapper>>(type, (_, __) => new() { InstanceBehavior = StubBehaviors.NotImplemented });

        ReaderStubProvider = collection.BuildServiceProvider();
    }

    public StubIReader<ManipulationWrapper> GetReaderStub(string type) => ReaderStubProvider.GetRequiredKeyedService<StubIReader<ManipulationWrapper>>(type);

    public ManipulationWrapperGenericReader Build() => new(
        AssertStub,
        GetReaderStub(MetaAtchWrapperReader.TYPE),
        GetReaderStub(MetaAtrWrapperReader.TYPE),
        GetReaderStub(MetaEqdpWrapperReader.TYPE),
        GetReaderStub(MetaEqpWrapperReader.TYPE),
        GetReaderStub(MetaEstWrapperReader.TYPE),
        GetReaderStub(MetaGeqpWrapperReader.TYPE),
        GetReaderStub(MetaGmpWrapperReader.TYPE),
        GetReaderStub(MetaImcWrapperReader.TYPE),
        GetReaderStub(MetaRspWrapperReader.TYPE),
        GetReaderStub(MetaShpWrapperReader.TYPE),
        PluginLogStub
    );
}
