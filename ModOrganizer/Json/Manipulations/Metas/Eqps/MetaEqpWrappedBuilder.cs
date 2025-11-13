using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Manipulations.Metas.Eqps;

public class MetaEqpWrappedBuilder(IPluginLog pluginLog) : ManipulationWrapperBuilder<MetaEqp>(pluginLog, TYPE)
{
    public static readonly string TYPE = "Eqp";

    private MetaEqpBuilder MetaEqpBuilder { get; init; } = new(pluginLog);

    protected override bool TryBuildWrapped(JsonElement jsonElement, [NotNullWhen(true)] out MetaEqp? instance) => MetaEqpBuilder.TryBuild(jsonElement, out instance);
}
