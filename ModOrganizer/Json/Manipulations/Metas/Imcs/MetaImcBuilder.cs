using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Manipulations.Metas.Imcs;

public class MetaImcBuilder(IPluginLog pluginLog) : Builder<MetaImc>(pluginLog)
{
    private MetaImcEntryBuilder ImcEntryBuilder { get; init; } = new(pluginLog);
    private MetaImcIdentifierBuilder ImcIdentifierBuilder { get; init; } = new(pluginLog);

    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out MetaImc? instance)
    {
        instance = null;

        if (!Assert.IsValue(jsonElement, JsonValueKind.Object)) return false;

        if (!Assert.IsPropertyPresent(jsonElement, nameof(MetaImc.Entry), out var entryProperty)) return false;

        if (!ImcEntryBuilder.TryBuild(entryProperty, out var entry))
        {
            PluginLog.Debug($"Failed to build [{nameof(MetaImcEntry)}] for [{nameof(MetaImc)}]:\n\t{entryProperty}");
            return false;
        }

        if (!ImcIdentifierBuilder.TryBuild(jsonElement, out var identifier))
        {
            PluginLog.Debug($"Failed to build base [{nameof(MetaImcIdentifier)}] for [{nameof(MetaImc)}]:\n\t{jsonElement}");
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
