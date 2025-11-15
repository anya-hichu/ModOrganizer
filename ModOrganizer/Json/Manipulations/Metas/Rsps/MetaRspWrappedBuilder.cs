using Dalamud.Plugin.Services;
using ModOrganizer.Json.Manipulations.Metas.Eqdps;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Manipulations.Metas.Rsps;

public class MetaRspWrappedBuilder(IPluginLog pluginLog) : ManipulationWrapperBuilder<MetaRsp>(pluginLog, TYPE)
{
    public static readonly string TYPE = "Rsp";

    private MetaRspBuilder MetaRspBuilder { get; init; } = new(pluginLog);

    protected override bool TryBuildWrapped(JsonElement jsonElement, [NotNullWhen(true)] out MetaRsp? instance) => MetaRspBuilder.TryBuild(jsonElement, out instance);
}
