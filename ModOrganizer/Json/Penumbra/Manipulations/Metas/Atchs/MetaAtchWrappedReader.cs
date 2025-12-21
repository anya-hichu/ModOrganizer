using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers.Penumbra.Manipulations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Readers.Penumbra.Manipulations.Metas.Atchs;

public class MetaAtchWrappedReader(IPluginLog pluginLog) : ManipulationWrapperReader<MetaAtch>(pluginLog, TYPE)
{
    public static readonly string TYPE = "Atch";

    private MetaAtchReader MetaAtchReader { get; init; } = new(pluginLog);

    protected override bool TryReadWrapped(JsonElement jsonElement, [NotNullWhen(true)] out MetaAtch? instance) => MetaAtchReader.TryRead(jsonElement, out instance);
}
