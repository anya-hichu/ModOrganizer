using Dalamud.Plugin.Services;
using ModOrganizer.Json.Asserts;
using ModOrganizer.Json.Readers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Groups;

public class GroupReader(IAssert assert, IPluginLog pluginLog) : Reader<Group>(assert, pluginLog), IBaseGroupReader
{
    private static readonly uint SUPPORTED_VERSION = 0;

    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out Group? instance)
    {
        instance = null;

        if (!Assert.IsValue(element, JsonValueKind.Object)) return false;

        uint? version = element.TryGetProperty(nameof(Group.Version), out var versionProperty) ? versionProperty.GetUInt32() : null;
        if (version != null && version != SUPPORTED_VERSION)
        {
            PluginLog.Warning($"Failed to read [{typeof(Group).Name}], unsupported [{nameof(Group.Version)}] found [{version}] (supported version: {SUPPORTED_VERSION}): {element}");
            return false;
        }

        if (!Assert.IsValuePresent(element, nameof(Group.Name), out var name)) return false;
        if (!Assert.IsValuePresent(element, nameof(Group.Type), out var type)) return false;

        var description = element.TryGetProperty(nameof(Group.Description), out var descriptionProperty) ? descriptionProperty.GetString() : null;
        var image = element.TryGetProperty(nameof(Group.Image), out var îmageProperty) ? îmageProperty.GetString() : null;

        int? page = element.TryGetProperty(nameof(Group.Page), out var pageProperty) && 
            pageProperty.TryGetInt32(out var parsedPage) ? parsedPage : null;
        int? priority = element.TryGetProperty(nameof(Group.Page), out var priorityProperty) && 
            priorityProperty.TryGetInt32(out var parsedPriority) ? parsedPriority : null;
        int? defaultSettings = element.TryGetProperty(nameof(Group.Page), out var defaultSettingsProperty) &&
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
