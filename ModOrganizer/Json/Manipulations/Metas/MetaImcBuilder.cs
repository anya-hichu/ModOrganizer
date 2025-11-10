using Dalamud.Plugin.Services;
using ModOrganizer.Json.Imcs;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Manipulations.Metas;

public class MetaImcBuilder(IPluginLog pluginLog) : Builder<MetaImc>(pluginLog)
{
    private ImcEntryBuilder ImcEntryBuilder { get; init; } = new(pluginLog);
    private ImcIdentifierBuilder ImcIdentifierBuilder { get; init; } = new(pluginLog);

    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out MetaImc? instance)
    {
        instance = default;

        if (!AssertHasProperty(jsonElement, nameof(MetaImc.Entry), out var entryProperty)) return false;

        if (!ImcEntryBuilder.TryBuild(entryProperty, out var entry))
        {
            PluginLog.Debug($"Failed to build [{nameof(ImcEntry)}] for [{nameof(MetaImc)}]");
            return false;
        }

        if (!ImcIdentifierBuilder.TryBuild(jsonElement, out var identifier))
        {
            PluginLog.Debug($"Failed to build base [{nameof(ImcIdentifier)}] for [{nameof(MetaImc)}]");
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
