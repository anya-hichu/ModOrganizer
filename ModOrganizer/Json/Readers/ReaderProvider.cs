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
        var collection = new ServiceCollection();

        AddRootSingletons(collection);
        AddContainerSingletons(collection);
        AddGroupSingletons(collection);
        AddManipulationSingletons(collection);
        AddOptionSingletons(collection);

        return collection.BuildServiceProvider();
    }

    private void AddRootSingletons(IServiceCollection collection)
    {
        collection
            .AddSingleton(pluginLog)

            .AddSingleton<IAssert, Assert>(p => new(p.GetRequiredService<IPluginLog>()))
            .AddSingleton<IElementReader, ElementReader>(p => new(p.GetRequiredService<IPluginLog>()))
            
            .AddSingleton<IDefaultModReader, DefaultModReader>(p => new(
                p.GetRequiredService<IAssert>(),
                p.GetRequiredService<IReader<Container>>(),
                p.GetRequiredService<IElementReader>(),
                p.GetRequiredService<IPluginLog>()
            ))
            
            .AddSingleton<ILocalModDataReader, LocalModDataReader>(p => new(
                p.GetRequiredService<IAssert>(),
                p.GetRequiredService<IElementReader>(),
                p.GetRequiredService<IPluginLog>()
            ))
            
            .AddSingleton<IModMetaReader, ModMetaReader>(p => new(
                p.GetRequiredService<IAssert>(),
                p.GetRequiredService<IElementReader>(),
                p.GetRequiredService<IPluginLog>()
            ))
            
            .AddSingleton<ISortOrderReader, SortOrderReader>(p => new(
                p.GetRequiredService<IAssert>(),
                p.GetRequiredService<IElementReader>(),
                p.GetRequiredService<IPluginLog>()
            ));
    }

    private static void AddContainerSingletons(IServiceCollection collection)
    {
        collection
            .AddSingleton<IReader<Container>, ContainerReader>(p => new(
                p.GetRequiredService<IAssert>(), 
                p.GetRequiredService<IManipulationWrapperReaderFactory>(), 
                p.GetRequiredService<IPluginLog>()
            ))
            .AddSingleton<IReader<NamedContainer>, NamedContainerReader>(p => new(
                p.GetRequiredService<IAssert>(), 
                p.GetRequiredService<IReader<Container>>(), 
                p.GetRequiredService<IPluginLog>()
            ));
    }

    private static void AddGroupSingletons(IServiceCollection collection)
    {
        collection
            .AddSingleton<IGroupBaseReader, GroupBaseReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()))
            
            .AddSingleton<IReader<Group>, GroupCombiningReader>(p => new(
                p.GetRequiredService<IAssert>(),
                p.GetRequiredService<IGroupBaseReader>(),
                p.GetRequiredService<IReader<NamedContainer>>(),
                p.GetRequiredService<IReader<Option>>(),
                p.GetRequiredService<IPluginLog>()
            ))
            
            .AddKeyedSingleton<IReader<Group>, GroupCombiningReader>(GroupCombiningReader.TYPE)
            
            .AddSingleton<IReader<Group>, GroupImcReader>(p => new(
                p.GetRequiredService<IAssert>(),
                p.GetRequiredService<IGroupBaseReader>(),
                p.GetRequiredService<IReader<MetaImcEntry>>(),
                p.GetRequiredService<IReader<MetaImcIdentifier>>(),
                p.GetRequiredService<IOptionImcReaderFactory>(),
                p.GetRequiredService<IPluginLog>()
            ))
            .AddKeyedSingleton<IReader<Group>, GroupImcReader>(GroupImcReader.TYPE)
            
            .AddSingleton<IReader<Group>, GroupMultiReader>(p => new(
                p.GetRequiredService<IAssert>(),
                p.GetRequiredService<IGroupBaseReader>(),
                p.GetRequiredService<IReader<OptionContainer>>(),
                p.GetRequiredService<IPluginLog>()
            ))
            .AddKeyedSingleton<IReader<Group>, GroupMultiReader>(GroupMultiReader.TYPE)

            .AddSingleton<IReader<Group>, GroupSingleReader>(p => new(
                p.GetRequiredService<IAssert>(),
                p.GetRequiredService<IGroupBaseReader>(),
                p.GetRequiredService<IReader<OptionContainer>>(),
                p.GetRequiredService<IPluginLog>()
            ))
            .AddKeyedSingleton<IReader<Group>, GroupSingleReader>(GroupSingleReader.TYPE)
            
            .AddSingleton<IGroupReaderFactory, GroupReaderFactory>(p => new(
                p.GetRequiredService<IAssert>(),
                p.GetRequiredKeyedService<IReader<Group>>(GroupCombiningReader.TYPE),
                p.GetRequiredKeyedService<IReader<Group>>(GroupImcReader.TYPE),
                p.GetRequiredKeyedService<IReader<Group>>(GroupMultiReader.TYPE),
                p.GetRequiredKeyedService<IReader<Group>>(GroupSingleReader.TYPE),
                p.GetRequiredService<IElementReader>(),
                p.GetRequiredService<IPluginLog>()
            ));
    }

    private static void AddManipulationSingletons(IServiceCollection collection)
    {
        collection
            .AddSingleton<IReader<MetaImcEntry>, MetaImcEntryReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()))
            .AddSingleton<IReader<MetaImcIdentifier>, MetaImcIdentifierReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()))
            
            .AddSingleton<IReader<MetaAtchEntry>, MetaAtchEntryReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()))
            .AddSingleton<IReader<MetaAtch>, MetaAtchReader>(p => new(
                p.GetRequiredService<IAssert>(), 
                p.GetRequiredService<IReader<MetaAtchEntry>>(), 
                p.GetRequiredService<IPluginLog>()
            ))
            .AddSingleton<IReader<ManipulationWrapper>, MetaAtchWrapperReader>(p => new(
                p.GetRequiredService<IAssert>(), 
                p.GetRequiredService<IReader<MetaAtch>>(), 
                p.GetRequiredService<IPluginLog>()
            ))
            .AddKeyedSingleton<IReader<ManipulationWrapper>, MetaAtrWrapperReader>(MetaAtchWrapperReader.TYPE)
            
            .AddSingleton<IReader<MetaAtr>, MetaAtrReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()))
            .AddSingleton<IReader<ManipulationWrapper>, MetaAtrWrapperReader>(p => new(
                p.GetRequiredService<IAssert>(), 
                p.GetRequiredService<IReader<MetaAtr>>(), 
                p.GetRequiredService<IPluginLog>()
            ))
            .AddKeyedSingleton<IReader<ManipulationWrapper>, MetaAtrWrapperReader>(MetaAtrWrapperReader.TYPE)
            
            .AddSingleton<IReader<MetaEqdp>, MetaEqdpReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()))
            .AddSingleton<IReader<ManipulationWrapper>, MetaEqdpWrapperReader>(p => new(
                p.GetRequiredService<IAssert>(), 
                p.GetRequiredService<IReader<MetaEqdp>>(), 
                p.GetRequiredService<IPluginLog>()
            ))
            .AddKeyedSingleton<IReader<ManipulationWrapper>, MetaEqdpWrapperReader>(MetaEqdpWrapperReader.TYPE)
            
            .AddSingleton<IReader<MetaEqp>, MetaEqpReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()))
            .AddSingleton<IReader<ManipulationWrapper>, MetaEqpWrapperReader>(p => new(
                p.GetRequiredService<IAssert>(), 
                p.GetRequiredService<IReader<MetaEqp>>(), 
                p.GetRequiredService<IPluginLog>()
            ))
            .AddKeyedSingleton<IReader<ManipulationWrapper>, MetaEqpWrapperReader>(MetaEqpWrapperReader.TYPE)
            
            .AddSingleton<IReader<MetaEst>, MetaEstReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()))
            .AddSingleton<IReader<ManipulationWrapper>, MetaEstWrapperReader>(p => new(
                p.GetRequiredService<IAssert>(), 
                p.GetRequiredService<IReader<MetaEst>>(), 
                p.GetRequiredService<IPluginLog>()
            ))
            .AddKeyedSingleton<IReader<ManipulationWrapper>, MetaEstWrapperReader>(MetaEstWrapperReader.TYPE)
            
            .AddSingleton<IReader<MetaGeqp>, MetaGeqpReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()))
            .AddSingleton<IReader<ManipulationWrapper>, MetaGeqpWrapperReader>(p => new(
                p.GetRequiredService<IAssert>(), 
                p.GetRequiredService<IReader<MetaGeqp>>(), 
                p.GetRequiredService<IPluginLog>()
            ))
            .AddKeyedSingleton<IReader<ManipulationWrapper>, MetaGeqpWrapperReader>(MetaGeqpWrapperReader.TYPE)
            
            .AddSingleton<IReader<MetaGmpEntry>, MetaGmpEntryReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()))
            .AddSingleton<IReader<MetaGmp>, MetaGmpReader>(p => new(
                p.GetRequiredService<IAssert>(), 
                p.GetRequiredService<IReader<MetaGmpEntry>>(), 
                p.GetRequiredService<IPluginLog>()
            ))
            .AddSingleton<IReader<ManipulationWrapper>, MetaGmpWrapperReader>(p => new(
                p.GetRequiredService<IAssert>(), 
                p.GetRequiredService<IReader<MetaGmp>>(), 
                p.GetRequiredService<IPluginLog>()
            ))
            .AddKeyedSingleton<IReader<ManipulationWrapper>, MetaGmpWrapperReader>(MetaGmpWrapperReader.TYPE)
            
            .AddSingleton<IReader<MetaImc>, MetaImcReader>(p => new(
                p.GetRequiredService<IAssert>(),
                p.GetRequiredService<IReader<MetaImcEntry>>(),
                p.GetRequiredService<IReader<MetaImcIdentifier>>(),
                p.GetRequiredService<IPluginLog>()
            ))
            .AddSingleton<IReader<ManipulationWrapper>, MetaImcWrapperReader>(p => new(
                p.GetRequiredService<IAssert>(), 
                p.GetRequiredService<IReader<MetaImc>>(), 
                p.GetRequiredService<IPluginLog>()
            ))
            .AddKeyedSingleton<IReader<ManipulationWrapper>, MetaImcWrapperReader>(MetaImcWrapperReader.TYPE)
            
            .AddSingleton<IReader<MetaRsp>, MetaRspReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()))
            .AddSingleton<IReader<ManipulationWrapper>, MetaRspWrapperReader>(p => new(
                p.GetRequiredService<IAssert>(), 
                p.GetRequiredService<IReader<MetaRsp>>(), 
                p.GetRequiredService<IPluginLog>()
            ))
            .AddKeyedSingleton<IReader<ManipulationWrapper>, MetaRspWrapperReader>(MetaRspWrapperReader.TYPE)
            
            .AddSingleton<IReader<MetaShp>, MetaShpReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()))
            .AddSingleton<IReader<ManipulationWrapper>, MetaShpWrapperReader>(p => new(
                p.GetRequiredService<IAssert>(), 
                p.GetRequiredService<IReader<MetaShp>>(), 
                p.GetRequiredService<IPluginLog>()
            ))
            .AddKeyedSingleton<IReader<ManipulationWrapper>, MetaShpWrapperReader>(MetaShpWrapperReader.TYPE)
            
            .AddSingleton<IManipulationWrapperReaderFactory, ManipulationWrapperReaderFactory>(p => new(
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

    private static void AddOptionSingletons(IServiceCollection collection)
    {
        collection
            .AddSingleton<IReader<Option>, OptionReader>(p => new(p.GetRequiredService<IAssert>(), p.GetRequiredService<IPluginLog>()))
            .AddSingleton<IReader<OptionContainer>, OptionContainerReader>(p => new(
                p.GetRequiredService<IAssert>(), 
                p.GetRequiredService<IReader<Container>>(), 
                p.GetRequiredService<IReader<Option>>(), 
                p.GetRequiredService<IPluginLog>()
            ))
            .AddSingleton<IOptionImcAttributeMaskReader, OptionImcAttributeMaskReader>(p => new(
                p.GetRequiredService<IAssert>(), 
                p.GetRequiredService<IReader<Option>>(), 
                p.GetRequiredService<IPluginLog>()
            ))
            .AddSingleton<IReader<OptionImc>, IOptionImcAttributeMaskReader>(p => p.GetRequiredService<IOptionImcAttributeMaskReader>())
            
            .AddSingleton<IOptionImcIsDisableSubModReader, OptionImcIsDisableSubModReader>(p => new(
                p.GetRequiredService<IAssert>(), 
                p.GetRequiredService<IReader<Option>>(), 
                p.GetRequiredService<IPluginLog>()
            ))
            .AddSingleton<IReader<OptionImc>, IOptionImcIsDisableSubModReader>(p => p.GetRequiredService<IOptionImcIsDisableSubModReader>())
            
            .AddSingleton<IOptionImcReaderFactory, OptionImcReaderFactory>(p => new(
                p.GetRequiredService<IAssert>(),
                p.GetRequiredService<IOptionImcAttributeMaskReader>(), 
                p.GetRequiredService<IOptionImcIsDisableSubModReader>(),
                p.GetRequiredService<IPluginLog>()
            ));
    }
}
