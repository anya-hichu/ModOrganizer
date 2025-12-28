using Dalamud.Plugin.Services;
using ModOrganizer.Json.Penumbra.Manipulations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Rsps;

public class MetaRspWrappedReader(IPluginLog pluginLog) : ManipulationWrapperReader<MetaRsp>(pluginLog, TYPE)
{
    public static readonly string TYPE = "Rsp";

    private MetaRspReader MetaRspReader { get; init; } = new(pluginLog);

    protected override bool TryReadWrapped(JsonElement jsonElement, [NotNullWhen(true)] out MetaRsp? instance) => MetaRspReader.TryRead(jsonElement, out instance);
}
