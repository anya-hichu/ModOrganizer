using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Manipulations.Metas.Imcs;

public class MetaImcEntryBuilder(IPluginLog pluginLog) : Builder<MetaImcEntry>(pluginLog)
{
    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out MetaImcEntry? instance)
    {
        instance = null;

        if (!AssertObject(jsonElement)) return false;

        if (!AssertU8PropertyValue(jsonElement, nameof(MetaImcEntry.MaterialId), out var materialId)) return false;
        if (!AssertU8PropertyValue(jsonElement, nameof(MetaImcEntry.DecalId), out var decalId)) return false;
        if (!AssertU8PropertyValue(jsonElement, nameof(MetaImcEntry.VfxId), out var vfxId)) return false;
        if (!AssertU8PropertyValue(jsonElement, nameof(MetaImcEntry.MaterialAnimationId), out var materialAnimationId)) return false;
        if (!AssertPropertyPresent(jsonElement, nameof(MetaImcEntry.AttributeMask), out var attributeMaskIdProperty)) return false;
        if (!AssertPropertyPresent(jsonElement, nameof(MetaImcEntry.SoundId), out var soundIdProperty)) return false;

        instance = new()
        {
            MaterialId = materialId,
            DecalId = decalId,
            VfxId = vfxId,
            MaterialAnimationId = materialAnimationId,
            AttributeMask = attributeMaskIdProperty.GetUInt16(),
            SoundId = soundIdProperty.GetByte()
        };

        return true;
    }
}
