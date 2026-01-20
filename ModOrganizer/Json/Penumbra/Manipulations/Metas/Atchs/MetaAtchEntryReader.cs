using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Elements;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Atchs;

public class MetaAtchEntryReader(IPluginLog pluginLog) : Reader<MetaAtchEntry>(pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out MetaAtchEntry? instance)
    {
        instance = null;

        if (!element.Is(JsonValueKind.Object, PluginLog)) return false;

        if (!element.TryGetRequiredNotEmptyPropertyValue(nameof(MetaAtchEntry.Bone), out var bone, PluginLog)) return false;
        if (!element.TryGetRequiredPropertyValue(nameof(MetaAtchEntry.Scale), out float scale, PluginLog)) return false;
        if (!element.TryGetRequiredPropertyValue(nameof(MetaAtchEntry.OffsetX), out float offsetX, PluginLog)) return false;
        if (!element.TryGetRequiredPropertyValue(nameof(MetaAtchEntry.OffsetY), out float offsetY, PluginLog)) return false;
        if (!element.TryGetRequiredPropertyValue(nameof(MetaAtchEntry.OffsetZ), out float offsetZ, PluginLog)) return false;
        if (!element.TryGetRequiredPropertyValue(nameof(MetaAtchEntry.RotationX), out float rotationX, PluginLog)) return false;
        if (!element.TryGetRequiredPropertyValue(nameof(MetaAtchEntry.RotationY), out float rotationY, PluginLog)) return false;
        if (!element.TryGetRequiredPropertyValue(nameof(MetaAtchEntry.RotationZ), out float rotationZ, PluginLog)) return false;

        instance = new()
        { 
            Bone = bone,
            Scale = scale,
            OffsetX = offsetX,
            OffsetY = offsetY,
            OffsetZ = offsetZ,
            RotationX = rotationX,
            RotationY = rotationY,
            RotationZ = rotationZ
        };

        return true;
    }
}
