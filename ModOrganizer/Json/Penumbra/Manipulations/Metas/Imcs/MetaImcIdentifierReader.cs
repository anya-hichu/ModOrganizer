using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Readers.Penumbra.Manipulations.Metas.Imcs;

public class MetaImcIdentifierReader(IPluginLog pluginLog) : Reader<MetaImcIdentifier>(pluginLog)
{
    public override bool TryRead(JsonElement jsonElement, [NotNullWhen(true)] out MetaImcIdentifier? instance)
    {
        instance = null;

        if (!Assert.IsValue(jsonElement, JsonValueKind.Object)) return false;

        if (!Assert.IsU16Value(jsonElement, nameof(MetaImcIdentifier.PrimaryId), out var primaryId)) return false;
        if (!Assert.IsU16Value(jsonElement, nameof(MetaImcIdentifier.SecondaryId), out var secondaryId)) return false;
        if (!Assert.IsU8Value(jsonElement, nameof(MetaImcIdentifier.Variant), out var variant)) return false;
        if (!Assert.IsValuePresent(jsonElement, nameof(MetaImcIdentifier.ObjectType), out var objectType)) return false;
        if (!Assert.IsValuePresent(jsonElement, nameof(MetaImcIdentifier.EquipSlot), out var equipSlot)) return false;
        if (!Assert.IsValuePresent(jsonElement, nameof(MetaImcIdentifier.BodySlot), out var bodySlot)) return false;

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
