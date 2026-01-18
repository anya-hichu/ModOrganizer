using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Groups;

public class GroupBaseReader(IPluginLog pluginLog) : Reader<Group>(pluginLog), IGroupBaseReader
{
    private static readonly uint SUPPORTED_VERSION = 0;

    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out Group? instance)
    {
        instance = null;

        if (!IsValue(element, JsonValueKind.Object)) return false;

        uint? version = element.TryGetProperty(nameof(Group.Version), out var versionProperty) ? versionProperty.GetUInt32() : null;
        if (version != null && version != SUPPORTED_VERSION)
        {
            PluginLog.Warning($"Failed to read [{typeof(Group).Name}], unsupported [{nameof(Group.Version)}] found [{version}] (supported version: {SUPPORTED_VERSION}): {element}");
            return false;
        }

        if (!TryGetRequiredValue(element, nameof(Group.Name), out var name)) return false;
        if (!TryGetRequiredValue(element, nameof(Group.Type), out var type)) return false;

        if (!TryGetOptionalValue(element, nameof(Group.Description), out string? description)) return false;
        if (!TryGetOptionalValue(element, nameof(Group.Image), out string? image)) return false;
        if (!TryGetOptionalValue(element, nameof(Group.Page), out int? page)) return false;
        if (!TryGetOptionalValue(element, nameof(Group.Priority), out int? priority)) return false;
        if (!TryGetOptionalValue(element, nameof(Group.DefaultSettings), out int? defaultSettings)) return false;

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
