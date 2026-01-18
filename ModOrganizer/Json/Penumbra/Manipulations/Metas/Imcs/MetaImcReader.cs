using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Imcs;

public class MetaImcReader(IReader<MetaImcEntry> imcEntryReader, IReader<MetaImcIdentifier> imcIdentifierReader, IPluginLog pluginLog) : Reader<MetaImc>(pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out MetaImc? instance)
    {
        instance = null;

        if (!IsValue(element, JsonValueKind.Object)) return false;

        if (!IsPropertyPresent(element, nameof(MetaImc.Entry), out var entryProperty)) return false;

        if (!imcEntryReader.TryRead(entryProperty, out var entry))
        {
            PluginLog.Debug($"Failed to read [{nameof(MetaImcEntry)}] for [{nameof(MetaImc)}]: {entryProperty}");
            return false;
        }

        if (!imcIdentifierReader.TryRead(element, out var identifier))
        {
            PluginLog.Debug($"Failed to read base [{nameof(MetaImcIdentifier)}] for [{nameof(MetaImc)}]: {element}");
            return false;
        }

        instance = new()
        {
            Entry = entry,

            PrimaryId = identifier.PrimaryId,
            SecondaryId = identifier.SecondaryId,
            Variant = identifier.Variant,
            ObjectType = identifier.ObjectType,
            EquipSlot = identifier.EquipSlot,
            BodySlot = identifier.BodySlot
        };

        return true;
    }
}
