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

        if (jsonElement.ValueKind != JsonValueKind.Object)
        {
            PluginLog.Warning($"Failed to build [{nameof(Option)}], expected root object but got [{jsonElement.ValueKind}]");
            return false;
        }

        if (!jsonElement.TryGetProperty(nameof(Option.Name), out var nameProperty))
        {
            PluginLog.Warning($"Failed to build [{nameof(Option)}], required attribute [{nameof(Option.Name)}] is missing");
            return false;
        }

        var name = nameProperty.GetString();
        if (name.IsNullOrEmpty())
        {
            PluginLog.Warning($"Failed to build [{nameof(Option)}], required attribute {nameof(Option.Name)} is null or empty");
            return false;
        }

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
