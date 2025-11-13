using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Manipulations.Metas.Imcs;

public class MetaImcWrappedBuilder(IPluginLog pluginLog) : ManipulationWrapperBuilder<MetaImc>(pluginLog, TYPE)
{
    public static readonly string TYPE = "Imc";

    private MetaImcBuilder MetaImcBuilder { get; init; } = new(pluginLog);

    protected override bool TryBuildWrapped(JsonElement jsonElement, [NotNullWhen(true)] out MetaImc? instance) => MetaImcBuilder.TryBuild(jsonElement, out instance);
}
