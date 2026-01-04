using Dalamud.Plugin.Services;
using ModOrganizer.Json.Asserts;
using ModOrganizer.Json.Readers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Eqdps;

public class MetaEqdpWrapperReader(IAssert assert, IReader<MetaEqdp> metaEqdpReader, IPluginLog pluginLog) : ManipulationWrapperReader<MetaEqdp>(assert, pluginLog, TYPE)
{
    public static readonly string TYPE = "Eqdp";

    protected override bool TryReadWrapped(JsonElement element, [NotNullWhen(true)] out MetaEqdp? instance) => metaEqdpReader.TryRead(element, out instance);
}
