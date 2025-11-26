using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Manipulations.Metas.Eqdps;

public class MetaEqdpWrappedBuilder(IPluginLog pluginLog) : ManipulationWrapperBuilder<MetaEqdp>(pluginLog, TYPE)
{
    public static readonly string TYPE = "Eqdp";

    private MetaEqdpBuilder MetaEqdpBuilder { get; init; } = new(pluginLog);

    protected override bool TryBuildWrapped(JsonElement jsonElement, [NotNullWhen(true)] out MetaEqdp? instance) => MetaEqdpBuilder.TryBuild(jsonElement, out instance);
}
