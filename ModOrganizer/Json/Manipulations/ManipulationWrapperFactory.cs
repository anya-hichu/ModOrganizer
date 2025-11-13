using Dalamud.Plugin.Services;
using ModOrganizer.Json.Manipulations.Metas.Atchs;
using ModOrganizer.Json.Manipulations.Metas.Atrs;
using ModOrganizer.Json.Manipulations.Metas.Eqdps;
using ModOrganizer.Json.Manipulations.Metas.Eqps;
using ModOrganizer.Json.Manipulations.Metas.Ests;
using ModOrganizer.Json.Manipulations.Metas.Geqps;
using ModOrganizer.Json.Manipulations.Metas.Imcs;

namespace ModOrganizer.Json.Manipulations;

public class ManipulationWrapperFactory : TypeFactory<ManipulationWrapper>
{
    public ManipulationWrapperFactory(IPluginLog pluginLog) : base(pluginLog)
    {
        Builders.Add(MetaImcWrappedBuilder.TYPE, new MetaImcWrappedBuilder(pluginLog));
        Builders.Add(MetaAtchWrappedBuilder.TYPE, new MetaAtchWrappedBuilder(pluginLog));
        Builders.Add(MetaAtrWrappedBuilder.TYPE, new MetaAtrWrappedBuilder(pluginLog));
        Builders.Add(MetaEqpdWrappedBuilder.TYPE, new MetaEqpdWrappedBuilder(pluginLog));
        Builders.Add(MetaEqpWrappedBuilder.TYPE, new MetaEqpWrappedBuilder(pluginLog));
        Builders.Add(MetaEstWrappedBuilder.TYPE, new MetaEstWrappedBuilder(pluginLog));
        Builders.Add(MetaGeqpWrappedBuilder.TYPE, new MetaGeqpWrappedBuilder(pluginLog));
    }
}
