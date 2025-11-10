using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Manipulations.Metas;

public class MetaAtrBuilder(IPluginLog pluginLog) : Builder<MetaAtr>(pluginLog)
{
    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out MetaAtr? instance)
    {
        throw new System.NotImplementedException();
    }
}
