using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Geqps;

public class MetaGeqpWrapperReader(IReader<MetaGeqp> metaGeqpReader, IPluginLog pluginLog) : ManipulationWrapperReader<MetaGeqp>(pluginLog, TYPE)
{
    public static readonly string TYPE = "GlobalEqp";

    protected override bool TryReadWrapped(JsonElement element, [NotNullWhen(true)] out MetaGeqp? instance) => metaGeqpReader.TryRead(element, out instance);
}
