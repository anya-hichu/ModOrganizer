using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Imcs;

public class ImcEntryBuilder(IPluginLog pluginLog) : Builder<ImcEntry>(pluginLog)
{
    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out ImcEntry? instance)
    {
        instance = default;

        if (!AssertIsObject(jsonElement)) return false;

        if (!AssertHasProperty(jsonElement, nameof(ImcEntry.MaterialId), out var materialIdProperty)) return false;
        if (!AssertHasProperty(jsonElement, nameof(ImcEntry.DecalId), out var decalIdProperty)) return false;
        if (!AssertHasProperty(jsonElement, nameof(ImcEntry.VfxId), out var vfxIdProperty)) return false;
        if (!AssertHasProperty(jsonElement, nameof(ImcEntry.MaterialAnimationId), out var materialAnimationIdProperty)) return false;
        if (!AssertHasProperty(jsonElement, nameof(ImcEntry.AttributeMask), out var attributeMaskIdProperty)) return false;
        if (!AssertHasProperty(jsonElement, nameof(ImcEntry.SoundId), out var soundIdProperty)) return false;

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
