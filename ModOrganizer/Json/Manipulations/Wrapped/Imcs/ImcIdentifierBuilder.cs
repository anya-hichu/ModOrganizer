using Dalamud.Plugin.Services;
using Dalamud.Utility;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Manipulations.Wrapped.Imcs;

public class ImcIdentifierBuilder(IPluginLog pluginLog) : Builder<ImcIdentifier>(pluginLog)
{
    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out ImcIdentifier? instance)
    {
        instance = default;

        if (jsonElement.ValueKind != JsonValueKind.Object)
        {
            PluginLog.Warning($"Failed to build [{nameof(ImcIdentifier)}], expected root object but got [{jsonElement.ValueKind}]");
            return false;
        }

        if (!jsonElement.TryGetProperty(nameof(ImcIdentifier.PrimaryId), out var primaryIdProperty))
        {
            PluginLog.Warning($"Failed to build [{nameof(ImcIdentifier)}], required attribute [{nameof(ImcIdentifier.PrimaryId)}] is missing");
            return false;
        }

        if (!jsonElement.TryGetProperty(nameof(ImcIdentifier.SecondaryId), out var secondaryIdProperty))
        {
            PluginLog.Warning($"Failed to build [{nameof(ImcIdentifier)}], required attribute [{nameof(ImcIdentifier.SecondaryId)}] is missing");
            return false;
        }

        if (!jsonElement.TryGetProperty(nameof(ImcIdentifier.Variant), out var variantProperty))
        {
            PluginLog.Warning($"Failed to build [{nameof(ImcIdentifier)}], required attribute [{nameof(ImcIdentifier.Variant)}] is missing");
            return false;
        }

        if (!jsonElement.TryGetProperty(nameof(ImcIdentifier.ObjectType), out var objectTypeProperty))
        {
            PluginLog.Warning($"Failed to build [{nameof(ImcIdentifier)}], required attribute [{nameof(ImcIdentifier.ObjectType)}] is missing");
            return false;
        }

        var objectType = objectTypeProperty.GetString();
        if (objectType.IsNullOrEmpty())
        {
            PluginLog.Warning($"Failed to build [{nameof(ImcIdentifier)}], required attribute [{nameof(ImcIdentifier.ObjectType)}] is null or empty");
            return false;
        }

        if (!jsonElement.TryGetProperty(nameof(ImcIdentifier.EquipSlot), out var equipSlotProperty))
        {
            PluginLog.Warning($"Failed to build [{nameof(ImcIdentifier)}], required attribute [{nameof(ImcIdentifier.EquipSlot)}] is missing");
            return false;
        }

        var equipSlot = equipSlotProperty.GetString();
        if (equipSlot.IsNullOrEmpty())
        {
            PluginLog.Warning($"Failed to build [{nameof(ImcIdentifier)}], required attribute [{nameof(ImcIdentifier.EquipSlot)}] is null or empty");
            return false;
        }

        if (!jsonElement.TryGetProperty(nameof(ImcIdentifier.BodySlot), out var bodySlotProperty))
        {
            PluginLog.Warning($"Failed to build [{nameof(ImcIdentifier)}], required attribute [{nameof(ImcIdentifier.BodySlot)}] is missing");
            return false;
        }

        var bodySlot = bodySlotProperty.GetString();
        if (bodySlot.IsNullOrEmpty())
        {
            PluginLog.Warning($"Failed to build [{nameof(ImcIdentifier)}], required attribute [{nameof(ImcIdentifier.BodySlot)}] is null or empty");
            return false;
        }

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
