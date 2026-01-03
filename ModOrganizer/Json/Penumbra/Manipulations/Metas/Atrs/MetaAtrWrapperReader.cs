using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Atrs;

public class MetaAtrWrapperReader(IReader<MetaAtr> metaAtrReader, IPluginLog pluginLog) : ManipulationWrapperReader<MetaAtr>(pluginLog, TYPE)
{
    public static readonly string TYPE = "Atr";

    protected override bool TryReadWrapped(JsonElement jsonElement, [NotNullWhen(true)] out MetaAtr? instance) => metaAtrReader.TryRead(jsonElement, out instance);
}
