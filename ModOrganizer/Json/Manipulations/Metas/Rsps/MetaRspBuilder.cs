using Dalamud.Plugin.Services;
using ModOrganizer.Json.Manipulations.Metas.Eqdps;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Manipulations.Metas.Rsps;

public class MetaRspBuilder(IPluginLog pluginLog) : Builder<MetaRsp>(pluginLog)
{
    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out MetaRsp? instance)
    {
        instance = null;

        if (!Assert.IsObject(jsonElement)) return false;

        if (!Assert.IsPropertyPresent(jsonElement, nameof(MetaRsp.Entry), out var entryProperty)) return false;
        if (!Assert.IsPropertyValuePresent(jsonElement, nameof(MetaRsp.SubRace), out var subRace)) return false;
        if (!Assert.IsPropertyValuePresent(jsonElement, nameof(MetaRsp.Attribute), out var attribute)) return false;

        instance = new()
        {
            Entry = entryProperty.GetSingle(),
            SubRace = subRace,
            Attribute = attribute
        };

        return true;
    }
}
