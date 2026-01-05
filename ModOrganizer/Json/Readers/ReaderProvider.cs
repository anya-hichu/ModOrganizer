using Dalamud.Plugin.Services;
using Microsoft.Extensions.DependencyInjection;
using ModOrganizer.Json.Penumbra.Containers;
using ModOrganizer.Json.Penumbra.DefaultMods;
using ModOrganizer.Json.Penumbra.Groups;
using ModOrganizer.Json.Penumbra.LocalModDatas;
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
using ModOrganizer.Json.Penumbra.ModMetas;
using ModOrganizer.Json.Penumbra.Options;
using ModOrganizer.Json.Penumbra.Options.Imcs;
using ModOrganizer.Json.Penumbra.SortOrders;
using ModOrganizer.Json.Readers.Asserts;
using ModOrganizer.Json.Readers.Elements;
using System;

namespace ModOrganizer.Json.Readers;

public class ReaderProvider : IDisposable
{
    private ServiceProvider ServiceProvider { get; init; }

    public ReaderProvider(IPluginLog pluginLog)
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton(pluginLog);
        serviceCollection.AddSingleton<IAssert, Assert>(p => new(p.GetRequiredService<IPluginLog>()));
        serviceCollection.AddSingleton<IElementReader, ElementReader>(p => new(p.GetRequiredService<IPluginLog>()));

        AddContainerSingletons(serviceCollection);
        AddGroupSingletons(serviceCollection);
        AddManipulationSingletons(serviceCollection);
        AddOptionSingletons(serviceCollection);

        serviceCollection.AddSingleton<IDefaultModReader, DefaultModReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IReader<Container>>(), 
            p.GetRequiredService<IElementReader>(), 
            p.GetRequiredService<IPluginLog>()
        ));

        serviceCollection.AddSingleton<ILocalModDataReader, LocalModDataReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IElementReader>(), 
            p.GetRequiredService<IPluginLog>()
        ));

        serviceCollection.AddSingleton<IModMetaReader, ModMetaReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IElementReader>(), 
            p.GetRequiredService<IPluginLog>()
        ));

        serviceCollection.AddSingleton<ISortOrderReader, SortOrderReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IElementReader>(), 
            p.GetRequiredService<IPluginLog>()
        ));

        ServiceProvider = serviceCollection.BuildServiceProvider();
    }

    public void Dispose() => ServiceProvider.Dispose();

    public T Get<T>() where T : notnull => ServiceProvider.GetRequiredService<T>();

    private static void AddContainerSingletons(ServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IReader<Container>, ContainerReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IManipulationWrapperReaderFactory>(), 
            p.GetRequiredService<IPluginLog>()
        ));
        serviceCollection.AddSingleton<IReader<NamedContainer>, NamedContainerReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IReader<Container>>(), 
            p.GetRequiredService<IPluginLog>()
        ));
    }

    private static void AddGroupSingletons(ServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IGroupBaseReader, GroupBaseReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()));

        serviceCollection.AddSingleton<IReader<Group>, GroupCombiningReader>(p => new(
            p.GetRequiredService<IAssert>(),
            p.GetRequiredService<IGroupBaseReader>(),
            p.GetRequiredService<IReader<NamedContainer>>(),
            p.GetRequiredService<IReader<Option>>(),
            p.GetRequiredService<IPluginLog>()
        ));
        serviceCollection.AddKeyedSingleton<IReader<Group>, GroupCombiningReader>(GroupCombiningReader.TYPE);

        serviceCollection.AddSingleton<IReader<Group>, GroupImcReader>(p => new(
            p.GetRequiredService<IAssert>(),
            p.GetRequiredService<IGroupBaseReader>(),
            p.GetRequiredService<IReader<MetaImcEntry>>(),
            p.GetRequiredService<IReader<MetaImcIdentifier>>(),
            p.GetRequiredService<IOptionImcReaderFactory>(),
            p.GetRequiredService<IPluginLog>()
        ));
        serviceCollection.AddKeyedSingleton<IReader<Group>, GroupImcReader>(GroupImcReader.TYPE);

        serviceCollection.AddSingleton<IReader<Group>, GroupMultiReader>(p => new(
            p.GetRequiredService<IAssert>(),
            p.GetRequiredService<IGroupBaseReader>(),
            p.GetRequiredService<IReader<OptionContainer>>(),
            p.GetRequiredService<IPluginLog>()
        ));
        serviceCollection.AddKeyedSingleton<IReader<Group>, GroupMultiReader>(GroupMultiReader.TYPE);

        serviceCollection.AddSingleton<IReader<Group>, GroupSingleReader>(p => new(
            p.GetRequiredService<IAssert>(),
            p.GetRequiredService<IGroupBaseReader>(),
            p.GetRequiredService<IReader<OptionContainer>>(),
            p.GetRequiredService<IPluginLog>()
        ));
        serviceCollection.AddKeyedSingleton<IReader<Group>, GroupSingleReader>(GroupSingleReader.TYPE);

        serviceCollection.AddSingleton<IGroupReaderFactory, GroupReaderFactory>(p => new(
            p.GetRequiredService<IAssert>(),
            p.GetRequiredKeyedService<IReader<Group>>(GroupCombiningReader.TYPE),
            p.GetRequiredKeyedService<IReader<Group>>(GroupImcReader.TYPE),
            p.GetRequiredKeyedService<IReader<Group>>(GroupMultiReader.TYPE),
            p.GetRequiredKeyedService<IReader<Group>>(GroupSingleReader.TYPE),
            p.GetRequiredService<IElementReader>(),
            p.GetRequiredService<IPluginLog>()
        ));
    }

    private static void AddManipulationSingletons(ServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IReader<MetaImcEntry>, MetaImcEntryReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()));
        serviceCollection.AddSingleton<IReader<MetaImcIdentifier>, MetaImcIdentifierReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()));

        serviceCollection.AddSingleton<IReader<MetaAtchEntry>, MetaAtchEntryReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()));
        serviceCollection.AddSingleton<IReader<MetaAtch>, MetaAtchReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IReader<MetaAtchEntry>>(), 
            p.GetRequiredService<IPluginLog>()
        ));
        serviceCollection.AddSingleton<IReader<ManipulationWrapper>, MetaAtchWrapperReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IReader<MetaAtch>>(), 
            p.GetRequiredService<IPluginLog>()
        ));
        serviceCollection.AddKeyedSingleton<IReader<ManipulationWrapper>, MetaAtrWrapperReader>(MetaAtchWrapperReader.TYPE);

        serviceCollection.AddSingleton<IReader<MetaAtr>, MetaAtrReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()));
        serviceCollection.AddSingleton<IReader<ManipulationWrapper>, MetaAtrWrapperReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IReader<MetaAtr>>(), 
            p.GetRequiredService<IPluginLog>()
        ));
        serviceCollection.AddKeyedSingleton<IReader<ManipulationWrapper>, MetaAtrWrapperReader>(MetaAtrWrapperReader.TYPE);

        serviceCollection.AddSingleton<IReader<MetaEqdp>, MetaEqdpReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()));
        serviceCollection.AddSingleton<IReader<ManipulationWrapper>, MetaEqdpWrapperReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IReader<MetaEqdp>>(), 
            p.GetRequiredService<IPluginLog>()
        ));
        serviceCollection.AddKeyedSingleton<IReader<ManipulationWrapper>, MetaEqdpWrapperReader>(MetaEqdpWrapperReader.TYPE);

        serviceCollection.AddSingleton<IReader<MetaEqp>, MetaEqpReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()));
        serviceCollection.AddSingleton<IReader<ManipulationWrapper>, MetaEqpWrapperReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IReader<MetaEqp>>(), 
            p.GetRequiredService<IPluginLog>()
        ));
        serviceCollection.AddKeyedSingleton<IReader<ManipulationWrapper>, MetaEqpWrapperReader>(MetaEqpWrapperReader.TYPE);

        serviceCollection.AddSingleton<IReader<MetaEst>, MetaEstReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()));
        serviceCollection.AddSingleton<IReader<ManipulationWrapper>, MetaEstWrapperReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IReader<MetaEst>>(), 
            p.GetRequiredService<IPluginLog>()
        ));
        serviceCollection.AddKeyedSingleton<IReader<ManipulationWrapper>, MetaEstWrapperReader>(MetaEstWrapperReader.TYPE);

        serviceCollection.AddSingleton<IReader<MetaGeqp>, MetaGeqpReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()));
        serviceCollection.AddSingleton<IReader<ManipulationWrapper>, MetaGeqpWrapperReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IReader<MetaGeqp>>(), 
            p.GetRequiredService<IPluginLog>()
        ));
        serviceCollection.AddKeyedSingleton<IReader<ManipulationWrapper>, MetaGeqpWrapperReader>(MetaGeqpWrapperReader.TYPE);

        serviceCollection.AddSingleton<IReader<MetaGmpEntry>, MetaGmpEntryReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()));
        serviceCollection.AddSingleton<IReader<MetaGmp>, MetaGmpReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IReader<MetaGmpEntry>>(), 
            p.GetRequiredService<IPluginLog>()
        ));
        serviceCollection.AddSingleton<IReader<ManipulationWrapper>, MetaGmpWrapperReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IReader<MetaGmp>>(), 
            p.GetRequiredService<IPluginLog>()
        ));
        serviceCollection.AddKeyedSingleton<IReader<ManipulationWrapper>, MetaGmpWrapperReader>(MetaGmpWrapperReader.TYPE);

        serviceCollection.AddSingleton<IReader<MetaImc>, MetaImcReader>(p => new(
            p.GetRequiredService<IAssert>(),
            p.GetRequiredService<IReader<MetaImcEntry>>(),
            p.GetRequiredService<IReader<MetaImcIdentifier>>(),
            p.GetRequiredService<IPluginLog>()
        ));
        serviceCollection.AddSingleton<IReader<ManipulationWrapper>, MetaImcWrapperReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IReader<MetaImc>>(), 
            p.GetRequiredService<IPluginLog>()
        ));
        serviceCollection.AddKeyedSingleton<IReader<ManipulationWrapper>, MetaImcWrapperReader>(MetaImcWrapperReader.TYPE);

        serviceCollection.AddSingleton<IReader<MetaRsp>, MetaRspReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()));
        serviceCollection.AddSingleton<IReader<ManipulationWrapper>, MetaRspWrapperReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IReader<MetaRsp>>(), 
            p.GetRequiredService<IPluginLog>()
        ));
        serviceCollection.AddKeyedSingleton<IReader<ManipulationWrapper>, MetaRspWrapperReader>(MetaRspWrapperReader.TYPE);

        serviceCollection.AddSingleton<IReader<MetaShp>, MetaShpReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()));
        serviceCollection.AddSingleton<IReader<ManipulationWrapper>, MetaShpWrapperReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IReader<MetaShp>>(), 
            p.GetRequiredService<IPluginLog>()
        ));
        serviceCollection.AddKeyedSingleton<IReader<ManipulationWrapper>, MetaShpWrapperReader>(MetaShpWrapperReader.TYPE);

        serviceCollection.AddSingleton<IManipulationWrapperReaderFactory, ManipulationWrapperReaderFactory>(p => new(
            p.GetRequiredService<IAssert>(),
            p.GetRequiredKeyedService<IReader<ManipulationWrapper>>(MetaAtchWrapperReader.TYPE),
            p.GetRequiredKeyedService<IReader<ManipulationWrapper>>(MetaAtrWrapperReader.TYPE),
            p.GetRequiredKeyedService<IReader<ManipulationWrapper>>(MetaEqdpWrapperReader.TYPE),
            p.GetRequiredKeyedService<IReader<ManipulationWrapper>>(MetaEqpWrapperReader.TYPE),
            p.GetRequiredKeyedService<IReader<ManipulationWrapper>>(MetaEstWrapperReader.TYPE),
            p.GetRequiredKeyedService<IReader<ManipulationWrapper>>(MetaGeqpWrapperReader.TYPE),
            p.GetRequiredKeyedService<IReader<ManipulationWrapper>>(MetaGmpWrapperReader.TYPE),
            p.GetRequiredKeyedService<IReader<ManipulationWrapper>>(MetaImcWrapperReader.TYPE),
            p.GetRequiredKeyedService<IReader<ManipulationWrapper>>(MetaRspWrapperReader.TYPE),
            p.GetRequiredKeyedService<IReader<ManipulationWrapper>>(MetaShpWrapperReader.TYPE),
            p.GetRequiredService<IPluginLog>()
        ));
    }

    private static void AddOptionSingletons(ServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IReader<Option>, OptionReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()));
        serviceCollection.AddSingleton<IReader<OptionContainer>, OptionContainerReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IReader<Container>>(), 
            p.GetRequiredService<IReader<Option>>(), 
            p.GetRequiredService<IPluginLog>()
        ));

        serviceCollection.AddSingleton<IOptionImcAttributeMaskReader, OptionImcAttributeMaskReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IReader<Option>>(), 
            p.GetRequiredService<IPluginLog>()
        ));
        serviceCollection.AddSingleton<IReader<OptionImc>, IOptionImcAttributeMaskReader>(p => p.GetRequiredService<IOptionImcAttributeMaskReader>());

        serviceCollection.AddSingleton<IOptionImcIsDisableSubModReader, OptionImcIsDisableSubModReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IReader<Option>>(), 
            p.GetRequiredService<IPluginLog>()
        ));
        serviceCollection.AddSingleton<IReader<OptionImc>, IOptionImcIsDisableSubModReader>(p => p.GetRequiredService<IOptionImcIsDisableSubModReader>());

        serviceCollection.AddSingleton<IOptionImcReaderFactory, OptionImcReaderFactory>(p => new(
            p.GetRequiredService<IAssert>(),
            p.GetRequiredService<IOptionImcAttributeMaskReader>(), 
            p.GetRequiredService<IOptionImcIsDisableSubModReader>(),
            p.GetRequiredService<IPluginLog>()
        ));
    }
}
