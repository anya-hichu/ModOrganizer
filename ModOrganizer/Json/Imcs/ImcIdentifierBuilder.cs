using Dalamud.Plugin.Services;
using Dalamud.Utility;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Imcs;

public class ImcIdentifierBuilder(IPluginLog pluginLog) : Builder<ImcIdentifier>(pluginLog)
{
    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out ImcIdentifier? instance)
    {
        instance = default;

        if (!AssertIsObject(jsonElement)) return false;

        if (!AssertHasProperty(jsonElement, nameof(ImcIdentifier.PrimaryId), out var primaryIdProperty)) return false;
        if (!AssertHasProperty(jsonElement, nameof(ImcIdentifier.SecondaryId), out var secondaryIdProperty)) return false;
        if (!AssertHasProperty(jsonElement, nameof(ImcIdentifier.Variant), out var variantProperty)) return false;
        if (!AssertStringPropertyPresent(jsonElement, nameof(ImcIdentifier.ObjectType), out var objectType)) return false;
        if (!AssertStringPropertyPresent(jsonElement, nameof(ImcIdentifier.EquipSlot), out var equipSlot)) return false;
        if (!AssertStringPropertyPresent(jsonElement, nameof(ImcIdentifier.BodySlot), out var bodySlot)) return false;

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
