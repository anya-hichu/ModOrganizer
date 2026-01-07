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
using ModOrganizer.Providers;

namespace ModOrganizer.Json.Readers;

public class ReaderProvider(IPluginLog pluginLog) : CachedProvider, IReaderProvider
{
    protected override ServiceProvider BuildServiceProvider()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton(pluginLog);

        AddRootSingletons(serviceCollection);
        AddContainerSingletons(serviceCollection);
        AddGroupSingletons(serviceCollection);
        AddManipulationSingletons(serviceCollection);
        AddOptionSingletons(serviceCollection);

        return serviceCollection.BuildServiceProvider();
    }

    private static void AddRootSingletons(ServiceCollection collection)
    {
        collection.AddSingleton<IAssert, Assert>(p => new(p.GetRequiredService<IPluginLog>()));
        collection.AddSingleton<IElementReader, ElementReader>(p => new(p.GetRequiredService<IPluginLog>()));

        collection.AddSingleton<IDefaultModReader, DefaultModReader>(p => new(
            p.GetRequiredService<IAssert>(),
            p.GetRequiredService<IReader<Container>>(),
            p.GetRequiredService<IElementReader>(),
            p.GetRequiredService<IPluginLog>()
        ));

        collection.AddSingleton<ILocalModDataReader, LocalModDataReader>(p => new(
            p.GetRequiredService<IAssert>(),
            p.GetRequiredService<IElementReader>(),
            p.GetRequiredService<IPluginLog>()
        ));

        collection.AddSingleton<IModMetaReader, ModMetaReader>(p => new(
            p.GetRequiredService<IAssert>(),
            p.GetRequiredService<IElementReader>(),
            p.GetRequiredService<IPluginLog>()
        ));

        collection.AddSingleton<ISortOrderReader, SortOrderReader>(p => new(
            p.GetRequiredService<IAssert>(),
            p.GetRequiredService<IElementReader>(),
            p.GetRequiredService<IPluginLog>()
        ));
    }

    private static void AddContainerSingletons(ServiceCollection collection)
    {
        collection.AddSingleton<IReader<Container>, ContainerReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IManipulationWrapperReaderFactory>(), 
            p.GetRequiredService<IPluginLog>()
        ));
        collection.AddSingleton<IReader<NamedContainer>, NamedContainerReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IReader<Container>>(), 
            p.GetRequiredService<IPluginLog>()
        ));
    }

    private static void AddGroupSingletons(ServiceCollection collection)
    {
        collection.AddSingleton<IGroupBaseReader, GroupBaseReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()));

        collection.AddSingleton<IReader<Group>, GroupCombiningReader>(p => new(
            p.GetRequiredService<IAssert>(),
            p.GetRequiredService<IGroupBaseReader>(),
            p.GetRequiredService<IReader<NamedContainer>>(),
            p.GetRequiredService<IReader<Option>>(),
            p.GetRequiredService<IPluginLog>()
        ));
        collection.AddKeyedSingleton<IReader<Group>, GroupCombiningReader>(GroupCombiningReader.TYPE);

        collection.AddSingleton<IReader<Group>, GroupImcReader>(p => new(
            p.GetRequiredService<IAssert>(),
            p.GetRequiredService<IGroupBaseReader>(),
            p.GetRequiredService<IReader<MetaImcEntry>>(),
            p.GetRequiredService<IReader<MetaImcIdentifier>>(),
            p.GetRequiredService<IOptionImcReaderFactory>(),
            p.GetRequiredService<IPluginLog>()
        ));
        collection.AddKeyedSingleton<IReader<Group>, GroupImcReader>(GroupImcReader.TYPE);

        collection.AddSingleton<IReader<Group>, GroupMultiReader>(p => new(
            p.GetRequiredService<IAssert>(),
            p.GetRequiredService<IGroupBaseReader>(),
            p.GetRequiredService<IReader<OptionContainer>>(),
            p.GetRequiredService<IPluginLog>()
        ));
        collection.AddKeyedSingleton<IReader<Group>, GroupMultiReader>(GroupMultiReader.TYPE);

        collection.AddSingleton<IReader<Group>, GroupSingleReader>(p => new(
            p.GetRequiredService<IAssert>(),
            p.GetRequiredService<IGroupBaseReader>(),
            p.GetRequiredService<IReader<OptionContainer>>(),
            p.GetRequiredService<IPluginLog>()
        ));
        collection.AddKeyedSingleton<IReader<Group>, GroupSingleReader>(GroupSingleReader.TYPE);

        collection.AddSingleton<IGroupReaderFactory, GroupReaderFactory>(p => new(
            p.GetRequiredService<IAssert>(),
            p.GetRequiredKeyedService<IReader<Group>>(GroupCombiningReader.TYPE),
            p.GetRequiredKeyedService<IReader<Group>>(GroupImcReader.TYPE),
            p.GetRequiredKeyedService<IReader<Group>>(GroupMultiReader.TYPE),
            p.GetRequiredKeyedService<IReader<Group>>(GroupSingleReader.TYPE),
            p.GetRequiredService<IElementReader>(),
            p.GetRequiredService<IPluginLog>()
        ));
    }

    private static void AddManipulationSingletons(ServiceCollection collection)
    {
        collection.AddSingleton<IReader<MetaImcEntry>, MetaImcEntryReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()));
        collection.AddSingleton<IReader<MetaImcIdentifier>, MetaImcIdentifierReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()));

        collection.AddSingleton<IReader<MetaAtchEntry>, MetaAtchEntryReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()));
        collection.AddSingleton<IReader<MetaAtch>, MetaAtchReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IReader<MetaAtchEntry>>(), 
            p.GetRequiredService<IPluginLog>()
        ));
        collection.AddSingleton<IReader<ManipulationWrapper>, MetaAtchWrapperReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IReader<MetaAtch>>(), 
            p.GetRequiredService<IPluginLog>()
        ));
        collection.AddKeyedSingleton<IReader<ManipulationWrapper>, MetaAtrWrapperReader>(MetaAtchWrapperReader.TYPE);

        collection.AddSingleton<IReader<MetaAtr>, MetaAtrReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()));
        collection.AddSingleton<IReader<ManipulationWrapper>, MetaAtrWrapperReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IReader<MetaAtr>>(), 
            p.GetRequiredService<IPluginLog>()
        ));
        collection.AddKeyedSingleton<IReader<ManipulationWrapper>, MetaAtrWrapperReader>(MetaAtrWrapperReader.TYPE);

        collection.AddSingleton<IReader<MetaEqdp>, MetaEqdpReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()));
        collection.AddSingleton<IReader<ManipulationWrapper>, MetaEqdpWrapperReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IReader<MetaEqdp>>(), 
            p.GetRequiredService<IPluginLog>()
        ));
        collection.AddKeyedSingleton<IReader<ManipulationWrapper>, MetaEqdpWrapperReader>(MetaEqdpWrapperReader.TYPE);

        collection.AddSingleton<IReader<MetaEqp>, MetaEqpReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()));
        collection.AddSingleton<IReader<ManipulationWrapper>, MetaEqpWrapperReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IReader<MetaEqp>>(), 
            p.GetRequiredService<IPluginLog>()
        ));
        collection.AddKeyedSingleton<IReader<ManipulationWrapper>, MetaEqpWrapperReader>(MetaEqpWrapperReader.TYPE);

        collection.AddSingleton<IReader<MetaEst>, MetaEstReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()));
        collection.AddSingleton<IReader<ManipulationWrapper>, MetaEstWrapperReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IReader<MetaEst>>(), 
            p.GetRequiredService<IPluginLog>()
        ));
        collection.AddKeyedSingleton<IReader<ManipulationWrapper>, MetaEstWrapperReader>(MetaEstWrapperReader.TYPE);

        collection.AddSingleton<IReader<MetaGeqp>, MetaGeqpReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()));
        collection.AddSingleton<IReader<ManipulationWrapper>, MetaGeqpWrapperReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IReader<MetaGeqp>>(), 
            p.GetRequiredService<IPluginLog>()
        ));
        collection.AddKeyedSingleton<IReader<ManipulationWrapper>, MetaGeqpWrapperReader>(MetaGeqpWrapperReader.TYPE);

        collection.AddSingleton<IReader<MetaGmpEntry>, MetaGmpEntryReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()));
        collection.AddSingleton<IReader<MetaGmp>, MetaGmpReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IReader<MetaGmpEntry>>(), 
            p.GetRequiredService<IPluginLog>()
        ));
        collection.AddSingleton<IReader<ManipulationWrapper>, MetaGmpWrapperReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IReader<MetaGmp>>(), 
            p.GetRequiredService<IPluginLog>()
        ));
        collection.AddKeyedSingleton<IReader<ManipulationWrapper>, MetaGmpWrapperReader>(MetaGmpWrapperReader.TYPE);

        collection.AddSingleton<IReader<MetaImc>, MetaImcReader>(p => new(
            p.GetRequiredService<IAssert>(),
            p.GetRequiredService<IReader<MetaImcEntry>>(),
            p.GetRequiredService<IReader<MetaImcIdentifier>>(),
            p.GetRequiredService<IPluginLog>()
        ));
        collection.AddSingleton<IReader<ManipulationWrapper>, MetaImcWrapperReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IReader<MetaImc>>(), 
            p.GetRequiredService<IPluginLog>()
        ));
        collection.AddKeyedSingleton<IReader<ManipulationWrapper>, MetaImcWrapperReader>(MetaImcWrapperReader.TYPE);

        collection.AddSingleton<IReader<MetaRsp>, MetaRspReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()));
        collection.AddSingleton<IReader<ManipulationWrapper>, MetaRspWrapperReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IReader<MetaRsp>>(), 
            p.GetRequiredService<IPluginLog>()
        ));
        collection.AddKeyedSingleton<IReader<ManipulationWrapper>, MetaRspWrapperReader>(MetaRspWrapperReader.TYPE);

        collection.AddSingleton<IReader<MetaShp>, MetaShpReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()));
        collection.AddSingleton<IReader<ManipulationWrapper>, MetaShpWrapperReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IReader<MetaShp>>(), 
            p.GetRequiredService<IPluginLog>()
        ));
        collection.AddKeyedSingleton<IReader<ManipulationWrapper>, MetaShpWrapperReader>(MetaShpWrapperReader.TYPE);

        collection.AddSingleton<IManipulationWrapperReaderFactory, ManipulationWrapperReaderFactory>(p => new(
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

    private static void AddOptionSingletons(ServiceCollection collection)
    {
        collection.AddSingleton<IReader<Option>, OptionReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()));
        collection.AddSingleton<IReader<OptionContainer>, OptionContainerReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IReader<Container>>(), 
            p.GetRequiredService<IReader<Option>>(), 
            p.GetRequiredService<IPluginLog>()
        ));

        collection.AddSingleton<IOptionImcAttributeMaskReader, OptionImcAttributeMaskReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IReader<Option>>(), 
            p.GetRequiredService<IPluginLog>()
        ));
        collection.AddSingleton<IReader<OptionImc>, IOptionImcAttributeMaskReader>(p => p.GetRequiredService<IOptionImcAttributeMaskReader>());

        collection.AddSingleton<IOptionImcIsDisableSubModReader, OptionImcIsDisableSubModReader>(p => new(
            p.GetRequiredService<IAssert>(), 
            p.GetRequiredService<IReader<Option>>(), 
            p.GetRequiredService<IPluginLog>()
        ));
        collection.AddSingleton<IReader<OptionImc>, IOptionImcIsDisableSubModReader>(p => p.GetRequiredService<IOptionImcIsDisableSubModReader>());

        collection.AddSingleton<IOptionImcReaderFactory, OptionImcReaderFactory>(p => new(
            p.GetRequiredService<IAssert>(),
            p.GetRequiredService<IOptionImcAttributeMaskReader>(), 
            p.GetRequiredService<IOptionImcIsDisableSubModReader>(),
            p.GetRequiredService<IPluginLog>()
        ));
    }
}
