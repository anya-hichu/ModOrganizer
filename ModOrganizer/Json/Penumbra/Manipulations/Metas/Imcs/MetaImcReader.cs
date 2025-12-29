using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Imcs;

public class MetaImcReader(IPluginLog pluginLog) : Reader<MetaImc>(pluginLog)
{
    private MetaImcEntryReader ImcEntryReader { get; init; } = new(pluginLog);
    private MetaImcIdentifierReader ImcIdentifierReader { get; init; } = new(pluginLog);

    public override bool TryRead(JsonElement jsonElement, [NotNullWhen(true)] out MetaImc? instance)
    {
        instance = null;

        if (!Assert.IsValue(jsonElement, JsonValueKind.Object)) return false;

        if (!Assert.IsPropertyPresent(jsonElement, nameof(MetaImc.Entry), out var entryProperty)) return false;

        if (!ImcEntryReader.TryRead(entryProperty, out var entry))
        {
            PluginLog.Debug($"Failed to read [{nameof(MetaImcEntry)}] for [{nameof(MetaImc)}]: {entryProperty}");
            return false;
        }

        if (!ImcIdentifierReader.TryRead(jsonElement, out var identifier))
        {
            PluginLog.Debug($"Failed to read base [{nameof(MetaImcIdentifier)}] for [{nameof(MetaImc)}]: {jsonElement}");
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
