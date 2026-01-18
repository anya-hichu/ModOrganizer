using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Elements;
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

        if (!TryGetRequiredDictValue(element, nameof(SortOrder.Data), out var data)) return false;
        if (!TryGetRequiredArrayValue(element, nameof(SortOrder.EmptyFolders), out string[]? emptyFolders)) return false;

        instance = new()
        { 
            Data = data,
            EmptyFolders = emptyFolders
        };

        return true;
    }
}
