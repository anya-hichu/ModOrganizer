using Dalamud.Plugin.Services;
using ModOrganizer.Json.Manipulations.Metas.Imcs;
using ModOrganizer.Json.Options.Imcs;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Groups;


public class GroupImcBuilder(IPluginLog pluginLog) : Builder<Group>(pluginLog)
{
    public static readonly string TYPE = "Imc";

    private GroupBuilder GroupBuilder { get; init; } = new(pluginLog);

    private MetaImcEntryBuilder ImcEntryBuilder { get; init; } = new(pluginLog);
    private MetaImcIdentifierBuilder ImcIdentifierBuilder { get; init; } = new(pluginLog);
    private OptionImcFactory OptionImcFactory { get; init; } = new(pluginLog);

    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out Group? instance)
    {
        instance = null;

        var allVariants = jsonElement.TryGetProperty(nameof(GroupImc.AllVariants), out var allVariantsProperty) && allVariantsProperty.GetBoolean();
        var allAttributes = jsonElement.TryGetProperty(nameof(GroupImc.AllAttributes), out var allAttributesProperty) && allAttributesProperty.GetBoolean();

        MetaImcIdentifier? identifier = null;
        if (jsonElement.TryGetProperty(nameof(GroupImc.Identifier), out var identifierProperty) && !ImcIdentifierBuilder.TryBuild(identifierProperty, out identifier))
        {
            PluginLog.Warning($"Failed to build [{nameof(MetaImcIdentifier)}] for [{nameof(GroupImc)}]:\n\t{identifierProperty}");
            return false;
        }

        MetaImcEntry? defaultEntry = null;
        if (jsonElement.TryGetProperty(nameof(GroupImc.DefaultEntry), out var defaultEntryProperty) && !ImcEntryBuilder.TryBuild(defaultEntryProperty, out defaultEntry))
        {
            PluginLog.Warning($"Failed to build [{nameof(MetaImcEntry)}] for [{nameof(GroupImc)}]:\n\t{defaultEntryProperty}");
            return false;
        }

        if (!GroupBuilder.TryBuild(jsonElement, out var group))
        {
            PluginLog.Debug($"Failed to build base [{nameof(Group)}] for [{nameof(GroupImc)}]:\n\t{jsonElement}");
            return false;
        }

        if (group.Type != TYPE)
        {
            PluginLog.Warning($"Failed to build [{nameof(GroupSingle)}], invalid type [{group.Type}] (expected: {TYPE}):\n\t{jsonElement}");
            return false;
        }

        var options = Array.Empty<OptionImc>();
        if (jsonElement.TryGetProperty(nameof(GroupImc.Options), out var optionsProperty) && !OptionImcFactory.TryBuildMany(optionsProperty, out options))
        {
            PluginLog.Warning($"Failed to build one or more [{nameof(OptionImc)}] for [{nameof(GroupImc)}]:\n\t{optionsProperty}");
            return false;
        }

        instance = new GroupImc()
        {
            Name = group.Name,
            Type = group.Type,
            Description = group.Description,
            Image = group.Image,
            Priority = group.Priority,
            DefaultSettings = group.DefaultSettings,

            AllVariants = allVariants,
            AllAttributes = allAttributes,
            Identifier = identifier,
            DefaultEntry = defaultEntry,
            Options = options
        };

        return true;
    }
}
