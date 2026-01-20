using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Elements;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Options;

public class OptionReader(IPluginLog pluginLog) : Reader<Option>(pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out Option? instance)
    {
        instance = null;

        if (!element.Is(JsonValueKind.Object, PluginLog)) return false;

        if (!element.TryGetRequiredPropertyValue(nameof(Option.Name), out string? name, PluginLog)) return false;
        if (!element.TryGetOptionalPropertyValue(nameof(Option.Description), out string? description, PluginLog)) return false;
        if (!element.TryGetOptionalPropertyValue(nameof(Option.Priority), out int? priority, PluginLog)) return false;
        if (!element.TryGetOptionalPropertyValue(nameof(Option.Image), out string? image, PluginLog)) return false;

        instance = new()
        { 
            Name = name,
            Description = description,
            Priority = priority,
            Image = image
        };

        return true;
    }
}
