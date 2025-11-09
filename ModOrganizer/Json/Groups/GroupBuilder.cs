using Dalamud.Plugin.Services;
using Dalamud.Utility;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Groups;

public class GroupBuilder(IPluginLog pluginLog) : Builder<Group>(pluginLog)
{
    private static readonly int SUPPORTED_VERSION = 0;

    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out Group? instance)
    {
        instance = default;

        if (jsonElement.ValueKind != JsonValueKind.Object)
        {
            PluginLog.Warning($"Failed to build [{typeof(Group).Name}], expected root object but got [{jsonElement.ValueKind}]");
            return false;
        }

        uint? version = jsonElement.TryGetProperty(nameof(Group.Version), out var versionProperty) ? versionProperty.GetUInt32() : null;
        if (version != null && version != SUPPORTED_VERSION)
        {
            PluginLog.Warning($"Failed to build [{typeof(Group).Name}], unsupported [{nameof(Group.Version)}] [{version}] (supported version: {SUPPORTED_VERSION})");
            return false;
        }

        if (!jsonElement.TryGetProperty(nameof(Group.Name), out var nameProperty))
        {
            PluginLog.Warning($"Failed to build [{typeof(Group).Name}], required attribute [{nameof(Group.Name)}] is missing");
            return false;
        }

        var name = nameProperty.GetString();
        if (name.IsNullOrEmpty())
        {
            PluginLog.Warning($"Failed to build [{typeof(Group).Name}], required attribute {nameof(Group.Name)} is null or empty");
            return false;
        }

        if (!jsonElement.TryGetProperty(nameof(Group.Type), out var typeProperty))
        {
            PluginLog.Warning($"Failed to build [{typeof(Group).Name}], required attribute [{nameof(Group.Type)}] is missing");
            return false;
        }

        var type = typeProperty.GetString();
        if (type == null)
        {
            PluginLog.Warning($"Failed to build [{typeof(Group).Name}], required attribute {nameof(Group.Name)} is null");
            return false;
        }

        var description = jsonElement.TryGetProperty(nameof(Group.Description), out var descriptionProperty) ? descriptionProperty.GetString() : null;
        var image = jsonElement.TryGetProperty(nameof(Group.Image), out var îmageProperty) ? îmageProperty.GetString() : null;

        int? page = jsonElement.TryGetProperty(nameof(Group.Page), out var pageProperty) && 
            pageProperty.TryGetInt32(out var parsedPage) ? parsedPage : null;
        int? priority = jsonElement.TryGetProperty(nameof(Group.Page), out var priorityProperty) && 
            priorityProperty.TryGetInt32(out var parsedPriority) ? parsedPriority : null;
        int? defaultSettings = jsonElement.TryGetProperty(nameof(Group.Page), out var defaultSettingsProperty) &&
            defaultSettingsProperty.TryGetInt32(out var parsedDefaultSettings) ? parsedDefaultSettings : null;

        instance = new Group()
        {
            Name = name,
            Type = type,
            Description = description,
            Image = image,
            Priority = priority,
            DefaultSettings = defaultSettings
        };

        return true;
    }
}
