using Dalamud.Plugin.Services;
using ModOrganizer.Json.Penumbra.Manipulations.Wrappers;
using ModOrganizer.Json.Readers;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Shps;

public class MetaShpWrapperReader(IReader<MetaShp> metaShpReader, IPluginLog pluginLog) : ManipulationWrapperReader<MetaShp>(pluginLog, TYPE)
{
    public static readonly string TYPE = "Shp";

    protected override bool TryReadWrapped(JsonElement element, [NotNullWhen(true)] out MetaShp? instance) => metaShpReader.TryRead(element, out instance);
}
