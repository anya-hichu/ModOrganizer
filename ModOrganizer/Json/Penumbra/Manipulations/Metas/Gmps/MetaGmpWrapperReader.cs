using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Gmps;

public class MetaGmpWrapperReader(IReader<MetaGmp> metaGmpReader, IPluginLog pluginLog) : ManipulationWrapperReader<MetaGmp>(pluginLog, TYPE)
{
    public static readonly string TYPE = "Gmp";

    protected override bool TryReadWrapped(JsonElement jsonElement, [NotNullWhen(true)] out MetaGmp? instance) => metaGmpReader.TryRead(jsonElement, out instance);
}
