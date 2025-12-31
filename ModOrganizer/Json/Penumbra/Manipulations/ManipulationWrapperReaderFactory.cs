using Dalamud.Plugin.Services;
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

public class ManipulationWrapperReaderFactory : TypeReaderFactory<ManipulationWrapper>
{
    public ManipulationWrapperReaderFactory(IPluginLog pluginLog) : base(pluginLog)
    {
        Readers.Add(MetaImcWrappedReader.TYPE, new MetaImcWrappedReader(pluginLog));
        Readers.Add(MetaAtchWrappedReader.TYPE, new MetaAtchWrappedReader(pluginLog));
        Readers.Add(MetaAtrWrappedReader.TYPE, new MetaAtrWrappedReader(pluginLog));
        Readers.Add(MetaEqdpWrappedReader.TYPE, new MetaEqdpWrappedReader(pluginLog));
        Readers.Add(MetaEqpWrappedReader.TYPE, new MetaEqpWrappedReader(pluginLog));
        Readers.Add(MetaEstWrappedReader.TYPE, new MetaEstWrappedReader(pluginLog));
        Readers.Add(MetaGeqpWrappedReader.TYPE, new MetaGeqpWrappedReader(pluginLog));
        Readers.Add(MetaGmpWrapperReader.TYPE, new MetaGmpWrapperReader(pluginLog));
        Readers.Add(MetaRspWrappedReader.TYPE, new MetaRspWrappedReader(pluginLog));
        Readers.Add(MetaShpWrappedReader.TYPE, new MetaShpWrappedReader(pluginLog));
    }
}
