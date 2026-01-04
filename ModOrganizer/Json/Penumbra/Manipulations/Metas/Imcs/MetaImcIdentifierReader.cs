using Dalamud.Plugin.Services;
using ModOrganizer.Json.Asserts;
using ModOrganizer.Json.Readers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Imcs;

public class MetaImcIdentifierReader(IAssert assert, IPluginLog pluginLog) : Reader<MetaImcIdentifier>(assert, pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out MetaImcIdentifier? instance)
    {
        instance = null;

        if (!Assert.IsValue(element, JsonValueKind.Object)) return false;

        if (!Assert.IsU16Value(element, nameof(MetaImcIdentifier.PrimaryId), out var primaryId)) return false;
        if (!Assert.IsU16Value(element, nameof(MetaImcIdentifier.SecondaryId), out var secondaryId)) return false;
        if (!Assert.IsU8Value(element, nameof(MetaImcIdentifier.Variant), out var variant)) return false;
        if (!Assert.IsValuePresent(element, nameof(MetaImcIdentifier.ObjectType), out var objectType)) return false;
        if (!Assert.IsValuePresent(element, nameof(MetaImcIdentifier.EquipSlot), out var equipSlot)) return false;
        if (!Assert.IsValuePresent(element, nameof(MetaImcIdentifier.BodySlot), out var bodySlot)) return false;

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
