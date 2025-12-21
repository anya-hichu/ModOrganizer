using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers.Penumbra.Manipulations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Readers.Penumbra.Manipulations.Metas.Eqdps;

public class MetaEqdpWrappedReader(IPluginLog pluginLog) : ManipulationWrapperReader<MetaEqdp>(pluginLog, TYPE)
{
    public static readonly string TYPE = "Eqdp";

    private MetaEqdpReader MetaEqdpReader { get; init; } = new(pluginLog);

    protected override bool TryReadWrapped(JsonElement jsonElement, [NotNullWhen(true)] out MetaEqdp? instance) => MetaEqdpReader.TryRead(jsonElement, out instance);
}
