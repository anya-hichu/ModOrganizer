using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Elements;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Groups;

public class GroupBaseReader(IPluginLog pluginLog) : Reader<Group>(pluginLog), IGroupBaseReader
{
    public static readonly uint SUPPORTED_VERSION = 0;

    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out Group? instance)
    {
        instance = null;

        if (!element.Is(JsonValueKind.Object, PluginLog)) return false;

        if (element.TryGetOptionalPropertyValue(nameof(Group.Version), out uint? version, PluginLog) && version != SUPPORTED_VERSION)
        {
            PluginLog.Warning($"Failed to read [{typeof(Group).Name}], unsupported [{nameof(Group.Version)}] found [{version}] (supported version: {SUPPORTED_VERSION}): {element}");
            return false;
        }

        if (!element.TryGetRequiredNotEmptyPropertyValue(nameof(Group.Name), out var name, PluginLog)) return false;
        if (!element.TryGetRequiredNotEmptyPropertyValue(nameof(Group.Type), out var type, PluginLog)) return false;

        if (!element.TryGetOptionalPropertyValue(nameof(Group.Description), out string? description, PluginLog)) return false;
        if (!element.TryGetOptionalPropertyValue(nameof(Group.Image), out string? image, PluginLog)) return false;
        if (!element.TryGetOptionalPropertyValue(nameof(Group.Page), out int? page, PluginLog)) return false;
        if (!element.TryGetOptionalPropertyValue(nameof(Group.Priority), out int? priority, PluginLog)) return false;
        if (!element.TryGetOptionalPropertyValue(nameof(Group.DefaultSettings), out int? defaultSettings, PluginLog)) return false;

        instance = new Group()
        {
            Version = version,
            Name = name,
            Description = description,
            Image = image,
            Page = page,
            Priority = priority,
            Type = type,
            DefaultSettings = defaultSettings
        };

        return true;
    }
}
