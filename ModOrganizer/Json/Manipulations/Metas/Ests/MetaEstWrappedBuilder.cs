using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Manipulations.Metas.Ests;

public class MetaEstWrappedBuilder(IPluginLog pluginLog) : ManipulationWrapperBuilder<MetaEst>(pluginLog, TYPE)
{
    public static readonly string TYPE = "Est";

    private MetaEstBuilder MetaEstBuilder { get; init; } = new(pluginLog);

    protected override bool TryBuildWrapped(JsonElement jsonElement, [NotNullWhen(true)] out MetaEst? instance) => MetaEstBuilder.TryBuild(jsonElement, out instance);
}
