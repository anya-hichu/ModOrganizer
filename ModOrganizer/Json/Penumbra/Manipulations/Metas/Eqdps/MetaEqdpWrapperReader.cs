using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Eqdps;

public class MetaEqdpWrapperReader(IReader<MetaEqdp> metaEqdpReader, IPluginLog pluginLog) : ManipulationWrapperReader<MetaEqdp>(pluginLog, TYPE)
{
    public static readonly string TYPE = "Eqdp";

    protected override bool TryReadWrapped(JsonElement jsonElement, [NotNullWhen(true)] out MetaEqdp? instance) => metaEqdpReader.TryRead(jsonElement, out instance);
}
