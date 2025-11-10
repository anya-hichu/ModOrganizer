using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Manipulations.Metas.Wrapped;

public class WrappedMetaImcBuilder(IPluginLog pluginLog) : ManipulationWrapperBuilder<MetaImc>(pluginLog, TYPE)
{
    public static readonly string TYPE = "Imc";

    private MetaImcBuilder MetaImcBuilder { get; init; } = new(pluginLog);

    protected override bool TryBuildWrapped(JsonElement jsonElement, [NotNullWhen(true)] out MetaImc? instance) => MetaImcBuilder.TryBuild(jsonElement, out instance);
}
