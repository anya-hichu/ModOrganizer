using Dalamud.Plugin.Services;
using ModOrganizer.Json.Manipulations.Wrapped;

namespace ModOrganizer.Json.Manipulations;

public class ManipulationFactory : Factory<ManipulationWrapper>
{
    public ManipulationFactory(IPluginLog pluginLog) : base(pluginLog)
    {
        Builders.Add(MetaUnknownBuilder.TYPE, new MetaUnknownBuilder(pluginLog));
        Builders.Add(MetaImcBuilder.TYPE, new MetaImcBuilder(pluginLog));
    }
}
