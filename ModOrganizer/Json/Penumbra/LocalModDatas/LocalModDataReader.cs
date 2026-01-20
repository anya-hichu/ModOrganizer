using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Elements;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.LocalModDatas;

public class LocalModDataReader(IElementReader elementReader, IPluginLog pluginLog) : Reader<LocalModData>(pluginLog), ILocalModDataReader
{
    public static readonly uint SUPPORTED_FILE_VERSION = 3;

    public IElementReader ElementReader { get; init; } = elementReader;

    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out LocalModData? instance)
    {
        instance = null;

        if (!element.Is(JsonValueKind.Object, PluginLog)) return false;

        if (!element.TryGetRequiredPropertyValue(nameof(LocalModData.FileVersion), out uint fileVersion, PluginLog)) return false;

        if (fileVersion != SUPPORTED_FILE_VERSION)
        {
            PluginLog.Warning($"Failed to read [{nameof(LocalModData)}], unsupported [{nameof(LocalModData.FileVersion)}] found [{fileVersion}] (supported version: {SUPPORTED_FILE_VERSION}): {element}");
            return false;
        }

        if (!element.TryGetOptionalPropertyValue(nameof(LocalModData.ImportDate), out long? importDate, PluginLog)) return false;
        if (!element.TryGetOptionalPropertyValue(nameof(LocalModData.LocalTags), out string[]? localTags, PluginLog)) return false;
        if (!element.TryGetOptionalPropertyValue(nameof(LocalModData.Favorite), out bool? favorite)) return false;
        if (!element.TryGetOptionalPropertyValue(nameof(LocalModData.PreferredChangedItems), out int[]? preferredChangedItems, PluginLog)) return false;

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
