using Dalamud.Plugin.Services;
using ModOrganizer.Json.Manipulations.Metas.Eqdps;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Manipulations.Metas.Shps;

public class MetaShpWrappedBuilder(IPluginLog pluginLog) : ManipulationWrapperBuilder<MetaShp>(pluginLog, TYPE)
{
    public static readonly string TYPE = "Shp";

    private MetaShpBuilder MetaShpBuilder { get; init; } = new(pluginLog);

    protected override bool TryBuildWrapped(JsonElement jsonElement, [NotNullWhen(true)] out MetaShp? instance) => MetaShpBuilder.TryBuild(jsonElement, out instance);
}
