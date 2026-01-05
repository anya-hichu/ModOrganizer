using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Asserts;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Gmps;

public class MetaGmpWrapperReader(IAssert assert, IReader<MetaGmp> metaGmpReader, IPluginLog pluginLog) : ManipulationWrapperReader<MetaGmp>(assert, pluginLog, TYPE)
{
    public static readonly string TYPE = "Gmp";

    protected override bool TryReadWrapped(JsonElement element, [NotNullWhen(true)] out MetaGmp? instance) => metaGmpReader.TryRead(element, out instance);
}
