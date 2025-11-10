using Dalamud.Plugin.Services;
using ModOrganizer.Json.Manipulations.Metas.Wrapped;

namespace ModOrganizer.Json.Manipulations;

public class ManipulationWrapperFactory : TypeFactory<ManipulationWrapper>
{
    public ManipulationWrapperFactory(IPluginLog pluginLog) : base(pluginLog)
    {
        Builders.Add(WrappedMetaImcBuilder.TYPE, new WrappedMetaImcBuilder(pluginLog));
        Builders.Add(WrappedMetaAtchBuilder.TYPE, new WrappedMetaAtchBuilder(pluginLog));
    }
}
