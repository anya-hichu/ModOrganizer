using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Manipulations.Metas.Geqps;

public class MetaGeqpWrappedBuilder(IPluginLog pluginLog) : ManipulationWrapperBuilder<MetaGeqp>(pluginLog, TYPE)
{
    public static readonly string TYPE = "GlobalEqp";

    private MetaGeqpBuilder MetaGeqpBuilder { get; init; } = new(pluginLog);

    protected override bool TryBuildWrapped(JsonElement jsonElement, [NotNullWhen(true)] out MetaGeqp? instance) => MetaGeqpBuilder.TryBuild(jsonElement, out instance);
}
