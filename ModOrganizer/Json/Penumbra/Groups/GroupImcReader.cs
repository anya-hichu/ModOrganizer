using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Imcs;
using ModOrganizer.Json.Penumbra.Options.Imcs;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using ModOrganizer.Json.Asserts;

namespace ModOrganizer.Json.Penumbra.Groups;


public class GroupImcReader(IAssert assert, IReader<Group> groupReader, IReader<MetaImcEntry> imcEntryReader, IReader<MetaImcIdentifier> imcIdentifierReader, IReader<OptionImc> optionImcReader,  IPluginLog pluginLog) : Reader<Group>(assert, pluginLog)
{
    public static readonly string TYPE = "Imc";

    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out Group? instance)
    {
        instance = null;

        var allVariants = element.TryGetProperty(nameof(GroupImc.AllVariants), out var allVariantsProperty) && allVariantsProperty.GetBoolean();
        var allAttributes = element.TryGetProperty(nameof(GroupImc.AllAttributes), out var allAttributesProperty) && allAttributesProperty.GetBoolean();

        MetaImcEntry? defaultEntry = null;
        if (element.TryGetProperty(nameof(GroupImc.DefaultEntry), out var defaultEntryProperty) && !imcEntryReader.TryRead(defaultEntryProperty, out defaultEntry))
        {
            PluginLog.Warning($"Failed to read [{nameof(MetaImcEntry)}] for [{nameof(GroupImc)}]: {defaultEntryProperty}");
            return false;
        }

        MetaImcIdentifier? identifier = null;
        if (element.TryGetProperty(nameof(GroupImc.Identifier), out var identifierProperty) && !imcIdentifierReader.TryRead(identifierProperty, out identifier))
        {
            PluginLog.Warning($"Failed to read [{nameof(MetaImcIdentifier)}] for [{nameof(GroupImc)}]: {identifierProperty}");
            return false;
        }

        if (!groupReader.TryRead(element, out var group))
        {
            PluginLog.Debug($"Failed to read base [{nameof(Group)}] for [{nameof(GroupImc)}]: {element}");
            return false;
        }

        if (group.Type != TYPE)
        {
            PluginLog.Warning($"Failed to read [{nameof(GroupSingle)}], invalid type [{group.Type}] (expected: {TYPE}): {element}");
            return false;
        }

        var options = Array.Empty<OptionImc>();
        if (element.TryGetProperty(nameof(GroupImc.Options), out var optionsProperty) && !optionImcReader.TryReadMany(optionsProperty, out options))
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
