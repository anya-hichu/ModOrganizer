using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Asserts;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Eqps;

public class MetaEqpWrapperReader(IAssert assert, IReader<MetaEqp> metaEqpReader, IPluginLog pluginLog) : ManipulationWrapperReader<MetaEqp>(assert, pluginLog, TYPE)
{
    public static readonly string TYPE = "Eqp";

    protected override bool TryReadWrapped(JsonElement element, [NotNullWhen(true)] out MetaEqp? instance) => metaEqpReader.TryRead(element, out instance);
}
