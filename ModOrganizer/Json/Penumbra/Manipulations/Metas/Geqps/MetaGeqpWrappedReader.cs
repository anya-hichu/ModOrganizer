using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers.Penumbra.Manipulations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Readers.Penumbra.Manipulations.Metas.Geqps;

public class MetaGeqpWrappedReader(IPluginLog pluginLog) : ManipulationWrapperReader<MetaGeqp>(pluginLog, TYPE)
{
    public static readonly string TYPE = "GlobalEqp";

    private MetaGeqpReader MetaGeqpReader { get; init; } = new(pluginLog);

    protected override bool TryReadWrapped(JsonElement jsonElement, [NotNullWhen(true)] out MetaGeqp? instance) => MetaGeqpReader.TryRead(jsonElement, out instance);
}
