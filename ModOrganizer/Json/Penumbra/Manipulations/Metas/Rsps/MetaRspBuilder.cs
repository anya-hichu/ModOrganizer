using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Readers.Penumbra.Manipulations.Metas.Rsps;

public class MetaRspReader(IPluginLog pluginLog) : Reader<MetaRsp>(pluginLog)
{
    public override bool TryRead(JsonElement jsonElement, [NotNullWhen(true)] out MetaRsp? instance)
    {
        instance = null;

        if (!Assert.IsValue(jsonElement, JsonValueKind.Object)) return false;

        if (!Assert.IsPropertyPresent(jsonElement, nameof(MetaRsp.Entry), out var entryProperty)) return false;
        if (!Assert.IsValuePresent(jsonElement, nameof(MetaRsp.SubRace), out var subRace)) return false;
        if (!Assert.IsValuePresent(jsonElement, nameof(MetaRsp.Attribute), out var attribute)) return false;

        instance = new()
        {
            Entry = entryProperty.GetSingle(),
            SubRace = subRace,
            Attribute = attribute
        };

        return true;
    }
}
