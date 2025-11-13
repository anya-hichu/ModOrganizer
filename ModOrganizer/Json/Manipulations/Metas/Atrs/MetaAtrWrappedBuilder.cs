using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Manipulations.Metas.Atrs;

public class MetaAtrWrappedBuilder(IPluginLog pluginLog) : ManipulationWrapperBuilder<MetaAtr>(pluginLog, TYPE)
{
    public static readonly string TYPE = "Atr";

    private MetaAtrBuilder MetaAtrBuilder { get; init; } = new(pluginLog);

    protected override bool TryBuildWrapped(JsonElement jsonElement, [NotNullWhen(true)] out MetaAtr? instance) => MetaAtrBuilder.TryBuild(jsonElement, out instance);
}
