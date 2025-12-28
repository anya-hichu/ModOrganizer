using Dalamud.Plugin.Services;
using ModOrganizer.Json.Penumbra.Manipulations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Imcs;

public class MetaImcWrappedReader(IPluginLog pluginLog) : ManipulationWrapperReader<MetaImc>(pluginLog, TYPE)
{
    public static readonly string TYPE = "Imc";

    private MetaImcReader MetaImcReader { get; init; } = new(pluginLog);

    protected override bool TryReadWrapped(JsonElement jsonElement, [NotNullWhen(true)] out MetaImc? instance) => MetaImcReader.TryRead(jsonElement, out instance);
}
