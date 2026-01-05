using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Asserts;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Shps;

public class MetaShpWrapperReader(IAssert assert, IReader<MetaShp> metaShpReader, IPluginLog pluginLog) : ManipulationWrapperReader<MetaShp>(assert, pluginLog, TYPE)
{
    public static readonly string TYPE = "Shp";

    protected override bool TryReadWrapped(JsonElement element, [NotNullWhen(true)] out MetaShp? instance) => metaShpReader.TryRead(element, out instance);
}
