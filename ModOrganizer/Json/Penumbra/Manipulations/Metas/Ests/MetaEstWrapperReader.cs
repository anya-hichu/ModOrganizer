using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Ests;

public class MetaEstWrapperReader(IReader<MetaEst> metaEstReader, IPluginLog pluginLog) : ManipulationWrapperReader<MetaEst>(pluginLog, TYPE)
{
    public static readonly string TYPE = "Est";

    protected override bool TryReadWrapped(JsonElement jsonElement, [NotNullWhen(true)] out MetaEst? instance) => metaEstReader.TryRead(jsonElement, out instance);
}
