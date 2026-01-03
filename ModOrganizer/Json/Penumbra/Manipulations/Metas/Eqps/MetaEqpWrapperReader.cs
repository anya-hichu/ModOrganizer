using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Eqps;

public class MetaEqpWrapperReader(IReader<MetaEqp> metaEqpReader, IPluginLog pluginLog) : ManipulationWrapperReader<MetaEqp>(pluginLog, TYPE)
{
    public static readonly string TYPE = "Eqp";

    protected override bool TryReadWrapped(JsonElement jsonElement, [NotNullWhen(true)] out MetaEqp? instance) => metaEqpReader.TryRead(jsonElement, out instance);
}
