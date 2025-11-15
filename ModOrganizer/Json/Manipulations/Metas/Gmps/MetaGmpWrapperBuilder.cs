using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Manipulations.Metas.Gmps;

public class MetaGmpWrapperBuilder(IPluginLog pluginLog) : ManipulationWrapperBuilder<MetaGmp>(pluginLog, TYPE)
{
    public static readonly string TYPE = "Gmp";

    private MetaGmpBuilder MetaGmpBuilder { get; init; } = new(pluginLog);

    protected override bool TryBuildWrapped(JsonElement jsonElement, [NotNullWhen(true)] out MetaGmp? instance) => MetaGmpBuilder.TryBuild(jsonElement, out instance);
}
