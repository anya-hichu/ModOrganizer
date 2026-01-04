using Dalamud.Plugin.Services;
using ModOrganizer.Json.Asserts;
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
using ModOrganizer.Json.Readers;

namespace ModOrganizer.Json.Penumbra.Manipulations;

public class ManipulationWrapperReaderFactory : TypeReaderFactory<ManipulationWrapper>, IManipulationWrapperReaderFactory
{
    public ManipulationWrapperReaderFactory(IAssert assert, IReader<ManipulationWrapper> metaAtchWrapperReader, IReader<ManipulationWrapper> metaAtrWrapperReader, 
        IReader<ManipulationWrapper> metaEqdpWrapperReader, IReader<ManipulationWrapper> metaEqpWrapperReader, IReader<ManipulationWrapper> metaEstWrapperReader, 
        IReader<ManipulationWrapper> metaGeqpWrapperReader, IReader<ManipulationWrapper> metaGmpWrapperReader, IReader<ManipulationWrapper> metaImcWrapperReader, 
        IReader<ManipulationWrapper> metaRspWrapperReader, IReader<ManipulationWrapper> metaShpWrapperReader, IPluginLog pluginLog) : base(assert, pluginLog)
    {
        Readers.Add(MetaAtchWrapperReader.TYPE, metaAtchWrapperReader);
        Readers.Add(MetaAtrWrapperReader.TYPE, metaAtrWrapperReader);
        Readers.Add(MetaEqdpWrapperReader.TYPE, metaEqdpWrapperReader);
        Readers.Add(MetaEqpWrapperReader.TYPE, metaEqpWrapperReader);
        Readers.Add(MetaEstWrapperReader.TYPE, metaEstWrapperReader);
        Readers.Add(MetaGeqpWrapperReader.TYPE, metaGeqpWrapperReader);
        Readers.Add(MetaGmpWrapperReader.TYPE, metaGmpWrapperReader);
        Readers.Add(MetaImcWrapperReader.TYPE, metaImcWrapperReader);
        Readers.Add(MetaRspWrapperReader.TYPE, metaRspWrapperReader);
        Readers.Add(MetaShpWrapperReader.TYPE, metaShpWrapperReader);
    }
}
