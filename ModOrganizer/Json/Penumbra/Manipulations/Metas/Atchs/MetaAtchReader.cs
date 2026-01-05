using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Asserts;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Atchs;

public class MetaAtchReader(IAssert assert, IReader<MetaAtchEntry> metaAtchEntryReader, IPluginLog pluginLog) : Reader<MetaAtch>(assert, pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out MetaAtch? instance)
    {
        instance = null;

        if (!Assert.IsValue(element, JsonValueKind.Object)) return false;

        if (!Assert.IsPropertyPresent(element, nameof(MetaAtch.Entry), out var entryProperty)) return false;
        if (!Assert.IsValuePresent(element, nameof(MetaAtch.Gender), out var gender)) return false;
        if (!Assert.IsValuePresent(element, nameof(MetaAtch.Race), out var race)) return false;
        if (!Assert.IsValuePresent(element, nameof(MetaAtch.Type), out var type)) return false;
        if (!Assert.IsU16Value(element, nameof(MetaAtch.Index), out var index)) return false;

        if (!metaAtchEntryReader.TryRead(entryProperty, out var entry))
        {
            PluginLog.Debug($"Failed to read [{nameof(MetaAtchEntry)}] for [{nameof(MetaAtch)}]: {entryProperty}");
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
