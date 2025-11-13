using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Manipulations.Metas.Atchs;

public class MetaAtchEntryBuilder(IPluginLog pluginLog) : Builder<MetaAtchEntry>(pluginLog)
{
    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out MetaAtchEntry? instance)
    {
        instance = null;

        if (!AssertObject(jsonElement)) return false;

        if (!AssertPropertyValuePresent(jsonElement, nameof(MetaAtchEntry.Bone), out var bone)) return false;

        if (!AssertPropertyPresent(jsonElement, nameof(MetaAtchEntry.Scale), out var scaleProperty)) return false;
        if (!AssertPropertyPresent(jsonElement, nameof(MetaAtchEntry.OffsetX), out var offsetXProperty)) return false;
        if (!AssertPropertyPresent(jsonElement, nameof(MetaAtchEntry.OffsetY), out var offsetYProperty)) return false;
        if (!AssertPropertyPresent(jsonElement, nameof(MetaAtchEntry.OffsetZ), out var offsetZProperty)) return false;
        if (!AssertPropertyPresent(jsonElement, nameof(MetaAtchEntry.RotationX), out var rotationXProperty)) return false;
        if (!AssertPropertyPresent(jsonElement, nameof(MetaAtchEntry.RotationY), out var rotationYProperty)) return false;
        if (!AssertPropertyPresent(jsonElement, nameof(MetaAtchEntry.RotationZ), out var rotationZProperty)) return false;

        instance = new()
        { 
            Bone = bone,
            Scale = scaleProperty.GetSingle(),
            OffsetX = offsetXProperty.GetSingle(),
            OffsetY = offsetYProperty.GetSingle(),
            OffsetZ = offsetZProperty.GetSingle(),
            RotationX = rotationXProperty.GetSingle(),
            RotationY = rotationYProperty.GetSingle(),
            RotationZ = rotationZProperty.GetSingle(),
        };

        return true;
    }
}
