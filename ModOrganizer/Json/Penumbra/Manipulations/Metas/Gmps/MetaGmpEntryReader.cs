using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Elements;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Gmps;

public class MetaGmpEntryReader(IPluginLog pluginLog) : Reader<MetaGmpEntry>(pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out MetaGmpEntry? instance)
    {
        instance = null;

        if (!element.Is(JsonValueKind.Object, PluginLog)) return false;

        if (!element.TryGetRequiredPropertyValue(nameof(MetaGmpEntry.Enabled), out bool enabled, PluginLog)) return false;
        if (!element.TryGetRequiredPropertyValue(nameof(MetaGmpEntry.Animated), out bool animated, PluginLog)) return false;
        if (!element.TryGetRequiredPropertyValue(nameof(MetaGmpEntry.RotationA), out ushort rotationA, PluginLog)) return false;
        if (!element.TryGetRequiredPropertyValue(nameof(MetaGmpEntry.RotationB), out ushort rotationB, PluginLog)) return false;
        if (!element.TryGetRequiredPropertyValue(nameof(MetaGmpEntry.RotationC), out ushort rotationC, PluginLog)) return false;
        if (!element.TryGetRequiredPropertyValue(nameof(MetaGmpEntry.UnknownA), out byte unknownA, PluginLog)) return false;
        if (!element.TryGetRequiredPropertyValue(nameof(MetaGmpEntry.UnknownB), out byte unknownB, PluginLog)) return false;

        instance = new()
        {
            Enabled = enabled,
            Animated = animated,
            RotationA = rotationA,
            RotationB = rotationB,
            RotationC = rotationC,
            UnknownA = unknownA,
            UnknownB = unknownB
        };

        return true;
    }
}
