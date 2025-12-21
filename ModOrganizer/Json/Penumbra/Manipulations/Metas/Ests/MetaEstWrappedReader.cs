using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers.Penumbra.Manipulations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Readers.Penumbra.Manipulations.Metas.Ests;

public class MetaEstWrappedReader(IPluginLog pluginLog) : ManipulationWrapperReader<MetaEst>(pluginLog, TYPE)
{
    public static readonly string TYPE = "Est";

    private MetaEstReader MetaEstReader { get; init; } = new(pluginLog);

    protected override bool TryReadWrapped(JsonElement jsonElement, [NotNullWhen(true)] out MetaEst? instance) => MetaEstReader.TryRead(jsonElement, out instance);
}
