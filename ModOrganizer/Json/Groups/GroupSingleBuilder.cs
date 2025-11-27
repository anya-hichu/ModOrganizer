using Dalamud.Plugin.Services;
using ModOrganizer.Json.Options;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;

namespace ModOrganizer.Json.Groups;

public class GroupSingleBuilder(IPluginLog pluginLog) : Builder<Group>(pluginLog)
{
    public static readonly string TYPE = "Single";

    private GroupBuilder GroupBuilder { get; init; } = new(pluginLog);
    private OptionContainerBuilder OptionContainerBuilder { get; init; } = new(pluginLog);

    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out Group? instance)
    {
        instance = null;

        if (!Assert.IsObject(jsonElement)) return false;

        if (!GroupBuilder.TryBuild(jsonElement, out var group))
        {
            PluginLog.Debug($"Failed to build base [{nameof(Group)}] for [{nameof(GroupSingle)}]:\n\t{jsonElement}");
            return false;
        }

        if (group.Type != TYPE)
        {
            PluginLog.Warning($"Failed to build [{nameof(GroupSingle)}], invalid type [{group.Type}] (expected: {TYPE}):\n\t{jsonElement}");
            return false;
        }

        var options = jsonElement.TryGetProperty(nameof(GroupSingle.Options), out var optionsProperty) ?
            optionsProperty.EnumerateArray().SelectMany<JsonElement, OptionContainer>(j => OptionContainerBuilder.TryBuild(j, out var optionContainer) ? [optionContainer] : []).ToArray() : [];

        instance = new GroupSingle()
        {
            Name = group.Name,
            Type = group.Type,
            Description = group.Description,
            Image = group.Image,
            Priority = group.Priority,
            DefaultSettings = group.DefaultSettings,
            Options = options
        };

        return true;
    }
}
