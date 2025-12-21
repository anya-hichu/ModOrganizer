using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers.Penumbra.Manipulations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Readers.Penumbra.Manipulations.Metas.Gmps;

public class MetaGmpWrapperReader(IPluginLog pluginLog) : ManipulationWrapperReader<MetaGmp>(pluginLog, TYPE)
{
    public static readonly string TYPE = "Gmp";

    private MetaGmpReader MetaGmpReader { get; init; } = new(pluginLog);

    protected override bool TryReadWrapped(JsonElement jsonElement, [NotNullWhen(true)] out MetaGmp? instance) => MetaGmpReader.TryRead(jsonElement, out instance);
}
