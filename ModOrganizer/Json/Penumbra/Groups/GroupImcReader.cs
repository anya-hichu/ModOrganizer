using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Imcs;
using ModOrganizer.Json.Penumbra.Options.Imcs;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Groups;


public class GroupImcReader(IReader<Group> groupReader, IReader<MetaImcEntry> imcEntryReader, IReader<MetaImcIdentifier> imcIdentifierReader, IReader<OptionImc> optionImcReader,  IPluginLog pluginLog) : Reader<Group>(pluginLog)
{
    public static readonly string TYPE = "Imc";

    public override bool TryRead(JsonElement jsonElement, [NotNullWhen(true)] out Group? instance)
    {
        instance = null;

        var allVariants = jsonElement.TryGetProperty(nameof(GroupImc.AllVariants), out var allVariantsProperty) && allVariantsProperty.GetBoolean();
        var allAttributes = jsonElement.TryGetProperty(nameof(GroupImc.AllAttributes), out var allAttributesProperty) && allAttributesProperty.GetBoolean();

        MetaImcEntry? defaultEntry = null;
        if (jsonElement.TryGetProperty(nameof(GroupImc.DefaultEntry), out var defaultEntryProperty) && !imcEntryReader.TryRead(defaultEntryProperty, out defaultEntry))
        {
            PluginLog.Warning($"Failed to read [{nameof(MetaImcEntry)}] for [{nameof(GroupImc)}]: {defaultEntryProperty}");
            return false;
        }

        MetaImcIdentifier? identifier = null;
        if (jsonElement.TryGetProperty(nameof(GroupImc.Identifier), out var identifierProperty) && !imcIdentifierReader.TryRead(identifierProperty, out identifier))
        {
            PluginLog.Warning($"Failed to read [{nameof(MetaImcIdentifier)}] for [{nameof(GroupImc)}]: {identifierProperty}");
            return false;
        }

        if (!groupReader.TryRead(jsonElement, out var group))
        {
            PluginLog.Debug($"Failed to read base [{nameof(Group)}] for [{nameof(GroupImc)}]: {jsonElement}");
            return false;
        }

        if (group.Type != TYPE)
        {
            PluginLog.Warning($"Failed to read [{nameof(GroupSingle)}], invalid type [{group.Type}] (expected: {TYPE}): {jsonElement}");
            return false;
        }

        var options = Array.Empty<OptionImc>();
        if (jsonElement.TryGetProperty(nameof(GroupImc.Options), out var optionsProperty) && !optionImcReader.TryReadMany(optionsProperty, out options))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(OptionImc)}] for [{nameof(GroupImc)}]: {optionsProperty}");
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
