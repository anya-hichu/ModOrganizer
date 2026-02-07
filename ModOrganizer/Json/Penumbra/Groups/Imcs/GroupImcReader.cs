using Dalamud.Plugin.Services;
using ModOrganizer.Json.Penumbra.Groups.Bases;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Imcs.Entries;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Imcs.Identifiers;
using ModOrganizer.Json.Penumbra.Options.Imcs;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Elements;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Groups.Imcs;

public class GroupImcReader(IGroupBaseReader groupBaseReader, IReader<MetaImcEntry> imcEntryReader, IReader<MetaImcIdentifier> imcIdentifierReader, IOptionImcGenericReader optionImcGenericReader,  IPluginLog pluginLog) : Reader<Group>(pluginLog)
{
    public static readonly string TYPE = "Imc";

    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out Group? instance)
    {
        instance = null;

        if (!element.Is(JsonValueKind.Object, PluginLog)) return false;

        if (!groupBaseReader.TryRead(element, out var baseGroup))
        {
            PluginLog.Debug($"Failed to read base [{nameof(Group)}] for [{nameof(GroupImc)}]: {element}");
            return false;
        }

        if (baseGroup.Type != TYPE)
        {
            PluginLog.Warning($"Failed to read [{nameof(GroupImc)}], invalid type [{baseGroup.Type}] (expected: {TYPE}): {element}");
            return false;
        }

        if (!element.TryGetOptionalPropertyValue(nameof(GroupImc.AllVariants), out bool? allVariants, PluginLog)) return false;
        if (!element.TryGetOptionalPropertyValue(nameof(GroupImc.AllAttributes), out bool? allAttributes, PluginLog)) return false;

        MetaImcEntry? defaultEntry = null;
        if (element.TryGetOptionalProperty(nameof(GroupImc.DefaultEntry), out var defaultEntryProperty, PluginLog) && !imcEntryReader.TryRead(defaultEntryProperty, out defaultEntry))
        {
            PluginLog.Warning($"Failed to read [{nameof(MetaImcEntry)}] for [{nameof(GroupImc)}]: {element}");
            return false;
        }

        MetaImcIdentifier? identifier = null;
        if (element.TryGetOptionalProperty(nameof(GroupImc.Identifier), out var identifierProperty, PluginLog) && !imcIdentifierReader.TryRead(identifierProperty, out identifier))
        {
            PluginLog.Warning($"Failed to read [{nameof(MetaImcIdentifier)}] for [{nameof(GroupImc)}]: {element}");
            return false;
        }

        OptionImc[]? options = null;
        if (element.TryGetOptionalProperty(nameof(GroupImc.Options), out var optionsProperty, PluginLog) && !optionImcGenericReader.TryReadMany(optionsProperty, out options))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(OptionImc)}] for [{nameof(GroupImc)}]: {element}");
            return false;
        }

        instance = new GroupImc()
        {
            Name = baseGroup.Name,
            Type = baseGroup.Type,
            Description = baseGroup.Description,
            Image = baseGroup.Image,
            Priority = baseGroup.Priority,
            DefaultSettings = baseGroup.DefaultSettings,

            AllVariants = allVariants,
            AllAttributes = allAttributes,
            Identifier = identifier,
            DefaultEntry = defaultEntry,
            Options = options
        };

        return true;
    }
}
