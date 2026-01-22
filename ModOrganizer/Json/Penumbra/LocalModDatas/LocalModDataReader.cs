using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Elements;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.LocalModDatas;

public class LocalModDataReader(IElementReader elementReader, IPluginLog pluginLog) : Reader<LocalModDataV3>(pluginLog), ILocalModDataReader
{
    public static readonly int SUPPORTED_FILE_VERSION = 3;

    public IElementReader ElementReader { get; init; } = elementReader;

    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out LocalModDataV3? instance)
    {
        instance = null;

        if (!element.Is(JsonValueKind.Object, PluginLog)) return false;

        if (!element.TryGetRequiredPropertyValue(nameof(LocalModDataV3.FileVersion), out uint fileVersion, PluginLog)) return false;

        if (fileVersion != SUPPORTED_FILE_VERSION)
        {
            PluginLog.Warning($"Failed to read [{nameof(LocalModDataV3)}], unsupported [{nameof(LocalModDataV3.FileVersion)}] found [{fileVersion}] (supported version: {SUPPORTED_FILE_VERSION}): {element}");
            return false;
        }

        if (!element.TryGetOptionalPropertyValue(nameof(LocalModDataV3.ImportDate), out long? importDate, PluginLog)) return false;
        if (!element.TryGetOptionalPropertyValue(nameof(LocalModDataV3.LocalTags), out string[]? localTags, PluginLog)) return false;
        if (!element.TryGetOptionalPropertyValue(nameof(LocalModDataV3.Favorite), out bool? favorite, PluginLog)) return false;
        if (!element.TryGetOptionalPropertyValue(nameof(LocalModDataV3.PreferredChangedItems), out int[]? preferredChangedItems, PluginLog)) return false;

        instance = new()
        {
            FileVersion = fileVersion,
            ImportDate = importDate,
            LocalTags = localTags,
            Favorite = favorite,
            PreferredChangedItems = preferredChangedItems
        };

        return true;
    }
}
