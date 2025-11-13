using Dalamud.Plugin.Services;
using Dalamud.Utility;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Manipulations.Metas.Imcs;

public class MetaImcIdentifierBuilder(IPluginLog pluginLog) : Builder<MetaImcIdentifier>(pluginLog)
{
    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out MetaImcIdentifier? instance)
    {
        instance = null;

        if (!AssertObject(jsonElement)) return false;

        if (!AssertU16PropertyValue(jsonElement, nameof(MetaImcIdentifier.PrimaryId), out var primaryId)) return false;
        if (!AssertU16PropertyValue(jsonElement, nameof(MetaImcIdentifier.SecondaryId), out var secondaryId)) return false;
        if (!AssertU8PropertyValue(jsonElement, nameof(MetaImcIdentifier.Variant), out var variant)) return false;
        if (!AssertPropertyValuePresent(jsonElement, nameof(MetaImcIdentifier.ObjectType), out var objectType)) return false;
        if (!AssertPropertyValuePresent(jsonElement, nameof(MetaImcIdentifier.EquipSlot), out var equipSlot)) return false;
        if (!AssertPropertyValuePresent(jsonElement, nameof(MetaImcIdentifier.BodySlot), out var bodySlot)) return false;

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
