using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Elements;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Ests;

public class MetaEstReader(IPluginLog pluginLog) : Reader<MetaEst>(pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out MetaEst? instance)
    {
        instance = null;

        if (!element.Is(JsonValueKind.Object, PluginLog)) return false;

        if (!element.TryGetRequiredU16PropertyValue(nameof(MetaEst.Entry), out var entry, PluginLog)) return false;
        if (!element.TryGetRequiredNotEmptyPropertyValue(nameof(MetaEst.Gender), out var gender, PluginLog)) return false;
        if (!element.TryGetRequiredNotEmptyPropertyValue(nameof(MetaEst.Race), out var race, PluginLog)) return false;
        if (!element.TryGetRequiredU16PropertyValue(nameof(MetaEst.SetId), out var setId, PluginLog)) return false;
        if (!element.TryGetRequiredNotEmptyPropertyValue(nameof(MetaEst.Slot), out var slot, PluginLog)) return false;

        instance = new()
        {
            Entry = entry,
            Gender = gender,
            Race = race,
            SetId = setId,
            Slot = slot
        };

        return true;
    }
}
