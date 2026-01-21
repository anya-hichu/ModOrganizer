using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Elements;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Atchs;

public class MetaAtchReader(IReader<MetaAtchEntry> metaAtchEntryReader, IPluginLog pluginLog) : Reader<MetaAtch>(pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out MetaAtch? instance)
    {
        instance = null;

        if (!element.Is(JsonValueKind.Object, PluginLog)) return false;

        if (!element.TryGetRequiredProperty(nameof(MetaAtch.Entry), out var entryProperty, PluginLog)) return false;
        if (!element.TryGetRequiredPropertyValue(nameof(MetaAtch.Gender), out string? gender, PluginLog)) return false;
        if (!element.TryGetRequiredPropertyValue(nameof(MetaAtch.Race), out string? race, PluginLog)) return false;
        if (!element.TryGetRequiredPropertyValue(nameof(MetaAtch.Type), out string? type, PluginLog)) return false;
        if (!element.TryGetRequiredU16PropertyValue(nameof(MetaAtch.Index), out var index, PluginLog)) return false;

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
