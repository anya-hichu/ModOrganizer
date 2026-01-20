using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Elements;
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

        if (!element.Is(JsonValueKind.Object, PluginLog)) return false;

        if (!element.TryGetRequiredPropertyValue(nameof(SortOrder.Data), out Dictionary<string, string>? data, PluginLog)) return false;
        if (!element.TryGetRequiredPropertyValue(nameof(SortOrder.EmptyFolders), out string[]? emptyFolders, PluginLog)) return false;

        instance = new()
        { 
            Data = data,
            EmptyFolders = emptyFolders
        };

        return true;
    }
}
