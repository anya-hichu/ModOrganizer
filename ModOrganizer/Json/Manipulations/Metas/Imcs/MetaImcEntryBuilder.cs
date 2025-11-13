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

        if (!AssertPropertyPresent(jsonElement, nameof(MetaImcEntry.MaterialId), out var materialIdProperty)) return false;
        if (!AssertPropertyPresent(jsonElement, nameof(MetaImcEntry.DecalId), out var decalIdProperty)) return false;
        if (!AssertPropertyPresent(jsonElement, nameof(MetaImcEntry.VfxId), out var vfxIdProperty)) return false;
        if (!AssertPropertyPresent(jsonElement, nameof(MetaImcEntry.MaterialAnimationId), out var materialAnimationIdProperty)) return false;
        if (!AssertPropertyPresent(jsonElement, nameof(MetaImcEntry.AttributeMask), out var attributeMaskIdProperty)) return false;
        if (!AssertPropertyPresent(jsonElement, nameof(MetaImcEntry.SoundId), out var soundIdProperty)) return false;

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
