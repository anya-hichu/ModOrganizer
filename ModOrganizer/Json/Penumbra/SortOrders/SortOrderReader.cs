using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;

using ModOrganizer.Json.Readers.Elements;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.SortOrders;

public class SortOrderReader(IElementReader elementReader, IPluginLog pluginLog) : Reader<SortOrder>(pluginLog), ISortOrderReader
{
    public IElementReader ElementReader { get; init; } = elementReader;

    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out SortOrder? instance)
    {
        instance = null;

        if (!IsValue(element, JsonValueKind.Object)) return false;

        var data = new Dictionary<string, string>();
        if (element.TryGetProperty(nameof(SortOrder.Data), out var dataProperty) && !IsDict(dataProperty, out data))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(SortOrder.Data)}] for [{nameof(SortOrder)}]: {element}");
            return false;
        }

        var emptyFolders = Array.Empty<string>();
        if (element.TryGetProperty(nameof(SortOrder.EmptyFolders), out var emptyFoldersProperty) && !IsArray(emptyFoldersProperty, out emptyFolders))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(SortOrder.EmptyFolders)}] for [{nameof(SortOrder)}]: {element}");
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
