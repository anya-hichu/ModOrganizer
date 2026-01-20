using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Elements;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Imcs;

public class MetaImcIdentifierReader(IPluginLog pluginLog) : Reader<MetaImcIdentifier>(pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out MetaImcIdentifier? instance)
    {
        instance = null;

        if (!element.Is(JsonValueKind.Object, PluginLog)) return false;

        if (!element.TryGetRequiredU16PropertyValue(nameof(MetaImcIdentifier.PrimaryId), out var primaryId, PluginLog)) return false;
        if (!element.TryGetRequiredU16PropertyValue(nameof(MetaImcIdentifier.SecondaryId), out var secondaryId, PluginLog)) return false;
        if (!element.TryGetRequiredU8PropertyValue(nameof(MetaImcIdentifier.Variant), out var variant, PluginLog)) return false;
        if (!element.TryGetRequiredNotEmptyPropertyValue(nameof(MetaImcIdentifier.ObjectType), out var objectType, PluginLog)) return false;
        if (!element.TryGetRequiredNotEmptyPropertyValue(nameof(MetaImcIdentifier.EquipSlot), out var equipSlot, PluginLog)) return false;
        if (!element.TryGetRequiredNotEmptyPropertyValue(nameof(MetaImcIdentifier.BodySlot), out var bodySlot, PluginLog)) return false;

        instance = new()
        {
            PrimaryId = primaryId,
            SecondaryId = secondaryId,
            Variant = variant,
            ObjectType = objectType,
            EquipSlot = equipSlot,
            BodySlot = bodySlot
        };

        return true;
    }
}
