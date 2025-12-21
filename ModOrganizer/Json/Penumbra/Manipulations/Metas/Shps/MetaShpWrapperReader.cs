using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers.Penumbra.Manipulations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Readers.Penumbra.Manipulations.Metas.Shps;

public class MetaShpWrappedReader(IPluginLog pluginLog) : ManipulationWrapperReader<MetaShp>(pluginLog, TYPE)
{
    public static readonly string TYPE = "Shp";

    private MetaShpReader MetaShpReader { get; init; } = new(pluginLog);

    protected override bool TryReadWrapped(JsonElement jsonElement, [NotNullWhen(true)] out MetaShp? instance) => MetaShpReader.TryRead(jsonElement, out instance);
}
