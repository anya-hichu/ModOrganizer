using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers.Penumbra.Manipulations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Readers.Penumbra.Manipulations.Metas.Eqps;

public class MetaEqpWrappedReader(IPluginLog pluginLog) : ManipulationWrapperReader<MetaEqp>(pluginLog, TYPE)
{
    public static readonly string TYPE = "Eqp";

    private MetaEqpReader MetaEqpReader { get; init; } = new(pluginLog);

    protected override bool TryReadWrapped(JsonElement jsonElement, [NotNullWhen(true)] out MetaEqp? instance) => MetaEqpReader.TryRead(jsonElement, out instance);
}
