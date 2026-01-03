using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Rsps;

public class MetaRspWrapperReader(IReader<MetaRsp> metaRspReader, IPluginLog pluginLog) : ManipulationWrapperReader<MetaRsp>(pluginLog, TYPE)
{
    public static readonly string TYPE = "Rsp";

    protected override bool TryReadWrapped(JsonElement jsonElement, [NotNullWhen(true)] out MetaRsp? instance) => metaRspReader.TryRead(jsonElement, out instance);
}
