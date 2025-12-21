using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers.Penumbra.Manipulations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Readers.Penumbra.Manipulations.Metas.Atrs;

public class MetaAtrWrappedReader(IPluginLog pluginLog) : ManipulationWrapperReader<MetaAtr>(pluginLog, TYPE)
{
    public static readonly string TYPE = "Atr";

    private MetaAtrReader MetaAtrReader { get; init; } = new(pluginLog);

    protected override bool TryReadWrapped(JsonElement jsonElement, [NotNullWhen(true)] out MetaAtr? instance) => MetaAtrReader.TryRead(jsonElement, out instance);
}
