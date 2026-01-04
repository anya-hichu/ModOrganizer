using Dalamud.Plugin.Services;
using ModOrganizer.Json.Asserts;
using ModOrganizer.Json.Readers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Atrs;

public class MetaAtrWrapperReader(IAssert assert, IReader<MetaAtr> metaAtrReader, IPluginLog pluginLog) : ManipulationWrapperReader<MetaAtr>(assert, pluginLog, TYPE)
{
    public static readonly string TYPE = "Atr";

    protected override bool TryReadWrapped(JsonElement element, [NotNullWhen(true)] out MetaAtr? instance) => metaAtrReader.TryRead(element, out instance);
}
