using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Readers.Penumbra.Manipulations.Metas.Atchs;

public class MetaAtchReader(IPluginLog pluginLog) : Reader<MetaAtch>(pluginLog)
{
    private MetaAtchEntryReader MetaAtchEntryReader { get; init; } = new(pluginLog);

    public override bool TryRead(JsonElement jsonElement, [NotNullWhen(true)] out MetaAtch? instance)
    {
        instance = null;

        if (!Assert.IsValue(jsonElement, JsonValueKind.Object)) return false;

        if (!Assert.IsPropertyPresent(jsonElement, nameof(MetaAtch.Entry), out var entryProperty)) return false;
        if (!Assert.IsValuePresent(jsonElement, nameof(MetaAtch.Gender), out var gender)) return false;
        if (!Assert.IsValuePresent(jsonElement, nameof(MetaAtch.Race), out var race)) return false;
        if (!Assert.IsValuePresent(jsonElement, nameof(MetaAtch.Type), out var type)) return false;
        if (!Assert.IsU16Value(jsonElement, nameof(MetaAtch.Index), out var index)) return false;

        if (!MetaAtchEntryReader.TryRead(entryProperty, out var entry))
        {
            PluginLog.Debug($"Failed to read [{nameof(MetaAtchEntry)}] for [{nameof(MetaAtch)}]:\n\t{entryProperty}");
            return false;
        }

        instance = new()
        {
            Entry = entry,
            Gender = gender,
            Race = race,
            Type = type,
            Index = index
        };

        return true;
    }
}
