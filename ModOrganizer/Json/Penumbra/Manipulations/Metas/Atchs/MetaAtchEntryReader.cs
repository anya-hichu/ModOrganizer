using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Readers.Penumbra.Manipulations.Metas.Atchs;

public class MetaAtchEntryReader(IPluginLog pluginLog) : Reader<MetaAtchEntry>(pluginLog)
{
    public override bool TryRead(JsonElement jsonElement, [NotNullWhen(true)] out MetaAtchEntry? instance)
    {
        instance = null;

        if (!Assert.IsValue(jsonElement, JsonValueKind.Object)) return false;

        if (!Assert.IsValuePresent(jsonElement, nameof(MetaAtchEntry.Bone), out var bone)) return false;

        if (!Assert.IsPropertyPresent(jsonElement, nameof(MetaAtchEntry.Scale), out var scaleProperty)) return false;
        if (!Assert.IsPropertyPresent(jsonElement, nameof(MetaAtchEntry.OffsetX), out var offsetXProperty)) return false;
        if (!Assert.IsPropertyPresent(jsonElement, nameof(MetaAtchEntry.OffsetY), out var offsetYProperty)) return false;
        if (!Assert.IsPropertyPresent(jsonElement, nameof(MetaAtchEntry.OffsetZ), out var offsetZProperty)) return false;
        if (!Assert.IsPropertyPresent(jsonElement, nameof(MetaAtchEntry.RotationX), out var rotationXProperty)) return false;
        if (!Assert.IsPropertyPresent(jsonElement, nameof(MetaAtchEntry.RotationY), out var rotationYProperty)) return false;
        if (!Assert.IsPropertyPresent(jsonElement, nameof(MetaAtchEntry.RotationZ), out var rotationZProperty)) return false;

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
