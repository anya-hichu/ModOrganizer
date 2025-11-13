using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Manipulations.Metas.Atchs;

public class MetaAtchWrappedBuilder(IPluginLog pluginLog) : ManipulationWrapperBuilder<MetaAtch>(pluginLog, TYPE)
{
    public static readonly string TYPE = "Atch";

    private MetaAtchBuilder MetaAtchBuilder { get; init; } = new(pluginLog);

    protected override bool TryBuildWrapped(JsonElement jsonElement, [NotNullWhen(true)] out MetaAtch? instance) => MetaAtchBuilder.TryBuild(jsonElement, out instance);
}
