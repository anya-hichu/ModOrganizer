using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Asserts;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Imcs;

public class MetaImcWrapperReader(IAssert assert, IReader<MetaImc> metaImcReader, IPluginLog pluginLog) : ManipulationWrapperReader<MetaImc>(assert, pluginLog, TYPE)
{
    public static readonly string TYPE = "Imc";

    protected override bool TryReadWrapped(JsonElement element, [NotNullWhen(true)] out MetaImc? instance) => metaImcReader.TryRead(element, out instance);
}
