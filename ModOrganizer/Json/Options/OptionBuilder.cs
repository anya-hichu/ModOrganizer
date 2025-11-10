using Dalamud.Plugin.Services;
using Dalamud.Utility;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Options;

public class OptionBuilder(IPluginLog pluginLog) : Builder<Option>(pluginLog)
{
    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out Option? instance)
    {
        instance = default;

        if (!AssertIsObject(jsonElement)) return false;

        if (!AssertStringPropertyPresent(jsonElement, nameof(Option.Name), out var name)) return false;

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
