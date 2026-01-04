using Dalamud.Plugin.Services;
using ModOrganizer.Json.Asserts;
using ModOrganizer.Json.Readers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Atchs;

public class MetaAtchWrapperReader(IAssert assert, IReader<MetaAtch> metaAtchReader, IPluginLog pluginLog) : ManipulationWrapperReader<MetaAtch>(assert, pluginLog, TYPE)
{
    public static readonly string TYPE = "Atch";

    protected override bool TryReadWrapped(JsonElement element, [NotNullWhen(true)] out MetaAtch? instance) => metaAtchReader.TryRead(element, out instance);
}
