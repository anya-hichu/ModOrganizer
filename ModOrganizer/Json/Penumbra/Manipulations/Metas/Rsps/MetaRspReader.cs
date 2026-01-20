using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Elements;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Rsps;

public class MetaRspReader(IPluginLog pluginLog) : Reader<MetaRsp>(pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out MetaRsp? instance)
    {
        instance = null;

        if (!element.Is(JsonValueKind.Object, PluginLog)) return false;

        if (!element.TryGetRequiredPropertyValue(nameof(MetaRsp.Entry), out float entry, PluginLog)) return false;
        if (!element.TryGetRequiredNotEmptyPropertyValue(nameof(MetaRsp.SubRace), out string? subRace, PluginLog)) return false;
        if (!element.TryGetRequiredNotEmptyPropertyValue(nameof(MetaRsp.Attribute), out string? attribute, PluginLog)) return false;

        instance = new()
        {
            Entry = entry,
            SubRace = subRace,
            Attribute = attribute
        };

        return true;
    }
}
