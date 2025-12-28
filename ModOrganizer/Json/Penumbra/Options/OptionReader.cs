using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Options;

public class OptionReader(IPluginLog pluginLog) : Reader<Option>(pluginLog)
{
    public override bool TryRead(JsonElement jsonElement, [NotNullWhen(true)] out Option? instance)
    {
        instance = null;

        if (!Assert.IsValue(jsonElement, JsonValueKind.Object)) return false;

        if (!Assert.IsValuePresent(jsonElement, nameof(Option.Name), out var name)) return false;

        var description = jsonElement.TryGetProperty(nameof(Option.Description), out var descriptionProperty) ? descriptionProperty.GetString() : null;
        int? priority = jsonElement.TryGetProperty(nameof(Option.Priority), out var priorityProperty) ? priorityProperty.GetInt32() : null;
        var image = jsonElement.TryGetProperty(nameof(Option.Image), out var imageProperty) ? imageProperty.GetString() : null;

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
