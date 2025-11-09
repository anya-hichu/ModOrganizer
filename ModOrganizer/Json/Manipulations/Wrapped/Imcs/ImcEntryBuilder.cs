using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Manipulations.Wrapped.Imcs;

public class ImcEntryBuilder(IPluginLog pluginLog) : Builder<ImcEntry>(pluginLog)
{
    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out ImcEntry? instance)
    {
        instance = default;

        if (jsonElement.ValueKind != JsonValueKind.Object)
        {
            PluginLog.Warning($"Failed to build [{nameof(ImcEntry)}], expected root object but got [{jsonElement.ValueKind}]");
            return false;
        }

        if (!jsonElement.TryGetProperty(nameof(ImcEntry.MaterialId), out var materialIdProperty))
        {
            PluginLog.Warning($"Failed to build [{nameof(ImcEntry)}], required attribute [{nameof(ImcEntry.MaterialId)}] is missing");
            return false;
        }

        if (!jsonElement.TryGetProperty(nameof(ImcEntry.DecalId), out var decalIdProperty))
        {
            PluginLog.Warning($"Failed to build [{nameof(ImcEntry)}], required attribute [{nameof(ImcEntry.DecalId)}] is missing");
            return false;
        }

        if (!jsonElement.TryGetProperty(nameof(ImcEntry.VfxId), out var vfxIdProperty))
        {
            PluginLog.Warning($"Failed to build [{nameof(ImcEntry)}], required attribute [{nameof(ImcEntry.VfxId)}] is missing");
            return false;
        }

        if (!jsonElement.TryGetProperty(nameof(ImcEntry.MaterialAnimationId), out var materialAnimationIdProperty))
        {
            PluginLog.Warning($"Failed to build [{nameof(ImcEntry)}], required attribute [{nameof(ImcEntry.MaterialAnimationId)}] is missing");
            return false;
        }

        if (!jsonElement.TryGetProperty(nameof(ImcEntry.AttributeMask), out var attributeMaskIdProperty))
        {
            PluginLog.Warning($"Failed to build [{nameof(ImcEntry)}], required attribute [{nameof(ImcEntry.AttributeMask)}] is missing");
            return false;
        }

        if (!jsonElement.TryGetProperty(nameof(ImcEntry.SoundId), out var soundIdProperty))
        {
            PluginLog.Warning($"Failed to build [{nameof(ImcEntry)}], required attribute [{nameof(ImcEntry.SoundId)}] is missing");
            return false;
        }

        instance = new()
        {
            MaterialId = materialIdProperty.GetByte(),
            DecalId = decalIdProperty.GetByte(),
            VfxId = vfxIdProperty.GetByte(),
            MaterialAnimationId = materialAnimationIdProperty.GetByte(),
            AttributeMask = attributeMaskIdProperty.GetUInt16(),
            SoundId = soundIdProperty.GetByte()
        };

        return true;
    }
}
