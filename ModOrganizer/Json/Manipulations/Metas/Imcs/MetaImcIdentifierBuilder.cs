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

        if (!AssertPropertyPresent(jsonElement, nameof(MetaImcIdentifier.PrimaryId), out var primaryIdProperty)) return false;
        if (!AssertPropertyPresent(jsonElement, nameof(MetaImcIdentifier.SecondaryId), out var secondaryIdProperty)) return false;
        if (!AssertPropertyPresent(jsonElement, nameof(MetaImcIdentifier.Variant), out var variantProperty)) return false;
        if (!AssertPropertyValuePresent(jsonElement, nameof(MetaImcIdentifier.ObjectType), out var objectType)) return false;
        if (!AssertPropertyValuePresent(jsonElement, nameof(MetaImcIdentifier.EquipSlot), out var equipSlot)) return false;
        if (!AssertPropertyValuePresent(jsonElement, nameof(MetaImcIdentifier.BodySlot), out var bodySlot)) return false;

        instance = new()
        {
            PrimaryId = primaryIdProperty.GetUInt16(),
            SecondaryId = secondaryIdProperty.GetUInt16(),
            Variant = variantProperty.GetByte(),
            ObjectType = objectType,
            EquipSlot = equipSlot,
            BodySlot = bodySlot
        };

        return true;
    }
}
