using Dalamud.Plugin.Services;
using ModOrganizer.Json.Asserts;
using ModOrganizer.Json.Readers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Rsps;

public class MetaRspWrapperReader(IAssert assert, IReader<MetaRsp> metaRspReader, IPluginLog pluginLog) : ManipulationWrapperReader<MetaRsp>(assert, pluginLog, TYPE)
{
    public static readonly string TYPE = "Rsp";

    protected override bool TryReadWrapped(JsonElement element, [NotNullWhen(true)] out MetaRsp? instance) => metaRspReader.TryRead(element, out instance);
}
