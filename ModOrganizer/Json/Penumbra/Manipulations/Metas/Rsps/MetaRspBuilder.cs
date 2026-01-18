using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Rsps;

public class MetaRspReader(IPluginLog pluginLog) : Reader<MetaRsp>(pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out MetaRsp? instance)
    {
        instance = null;

        if (!IsValue(element, JsonValueKind.Object)) return false;

        if (!TryGetRequiredProperty(element, nameof(MetaRsp.Entry), out var entryProperty)) return false;
        if (!TryGetRequiredValue(element, nameof(MetaRsp.SubRace), out var subRace)) return false;
        if (!TryGetRequiredValue(element, nameof(MetaRsp.Attribute), out var attribute)) return false;

        instance = new()
        {
            Entry = entryProperty.GetSingle(),
            SubRace = subRace,
            Attribute = attribute
        };

        return true;
    }
}
