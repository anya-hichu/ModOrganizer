using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers.Files;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Readers.Penumbra.SortOrders;

public class SortOrderReader(IPluginLog pluginLog) : Reader<SortOrder>(pluginLog), IReadableFile<SortOrder>
{
    public FileReader FileReader { get; init; } = new(pluginLog);

    public override bool TryRead(JsonElement jsonElement, [NotNullWhen(true)] out SortOrder? instance)
    {
        instance = null;

        if (!Assert.IsValue(jsonElement, JsonValueKind.Object)) return false;

        var data = new Dictionary<string, string>();
        if (jsonElement.TryGetProperty(nameof(SortOrder.Data), out var dataProperty) && !Assert.IsStringDict(dataProperty, out data))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(SortOrder.Data)}] for [{nameof(SortOrder)}]:\n\t{dataProperty}");
            return false;
        }

        var emptyFolders = Array.Empty<string>();
        if (jsonElement.TryGetProperty(nameof(SortOrder.EmptyFolders), out var emptyFoldersProperty) && !Assert.IsStringArray(emptyFoldersProperty, out emptyFolders))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(SortOrder.EmptyFolders)}] for [{nameof(SortOrder)}]:\n\t{emptyFoldersProperty}");
            return false;
        }

        instance = new()
        { 
            Data = data,
            EmptyFolders = emptyFolders
        };

        return true;
    }
}
