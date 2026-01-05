using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Asserts;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Ests;

public class MetaEstWrapperReader(IAssert assert, IReader<MetaEst> metaEstReader, IPluginLog pluginLog) : ManipulationWrapperReader<MetaEst>(assert, pluginLog, TYPE)
{
    public static readonly string TYPE = "Est";

    protected override bool TryReadWrapped(JsonElement element, [NotNullWhen(true)] out MetaEst? instance) => metaEstReader.TryRead(element, out instance);
}
