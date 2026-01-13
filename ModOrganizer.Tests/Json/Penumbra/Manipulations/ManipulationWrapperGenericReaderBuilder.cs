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
using ModOrganizer.Tests.Json.Readers.Asserts;

namespace ModOrganizer.Tests.Json.Penumbra.Manipulations;

public class ManipulationWrapperGenericReaderBuilder : IBuilder<ManipulationWrapperGenericReader>, IStubbableAssert, IStubbablePluginLog
{
    private static readonly HashSet<string> TYPES = [MetaAtchWrapperReader.TYPE, MetaAtrWrapperReader.TYPE, MetaEqdpWrapperReader.TYPE, MetaEqpWrapperReader.TYPE,
            MetaEstWrapperReader.TYPE, MetaGeqpWrapperReader.TYPE, MetaGmpWrapperReader.TYPE, MetaImcWrapperReader.TYPE, MetaRspWrapperReader.TYPE, MetaShpWrapperReader.TYPE];

    public StubIAssert AssertStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public ServiceProvider ReaderStubProvider { get; init; }

    public ManipulationWrapperGenericReaderBuilder()
    {
        var collection = new ServiceCollection();

        foreach (var type in TYPES) collection.AddKeyedSingleton<StubIReader<ManipulationWrapper>>(type, (_, __) => new() { InstanceBehavior = StubBehaviors.NotImplemented });

        ReaderStubProvider = collection.BuildServiceProvider();
    }

    private StubIReader<ManipulationWrapper> GetReaderStub(string type) => ReaderStubProvider.GetRequiredKeyedService<StubIReader<ManipulationWrapper>>(type);

    public ManipulationWrapperGenericReaderBuilder WithManipulationWrapperReaderTryRead(string type, ManipulationWrapper? stubValue)
    {
        GetReaderStub(type).TryReadJsonElementT0Out = (element, out instance) =>
        {
            instance = stubValue!;

            return stubValue != null;
        };

        return this;
    }

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
