using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Asserts;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Options;

public class OptionReader(IAssert assert, IPluginLog pluginLog) : Reader<Option>(assert, pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out Option? instance)
    {
        instance = null;

        if (!Assert.IsValue(element, JsonValueKind.Object)) return false;

        if (!Assert.IsValuePresent(element, nameof(Option.Name), out var name)) return false;

        var description = element.TryGetProperty(nameof(Option.Description), out var descriptionProperty) ? descriptionProperty.GetString() : null;
        int? priority = element.TryGetProperty(nameof(Option.Priority), out var priorityProperty) ? priorityProperty.GetInt32() : null;
        var image = element.TryGetProperty(nameof(Option.Image), out var imageProperty) ? imageProperty.GetString() : null;

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
