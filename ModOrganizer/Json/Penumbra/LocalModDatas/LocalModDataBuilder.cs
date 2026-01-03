using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Files;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.LocalModDatas;

public class LocalModDataReader(IFileReader fileReader, IPluginLog pluginLog) : Reader<LocalModData>(pluginLog), ILocalModDataReader
{
    private static readonly uint SUPPORTED_FILE_VERSION = 3;

    public IFileReader FileReader { get; init; } = fileReader;

    public override bool TryRead(JsonElement jsonElement, [NotNullWhen(true)] out LocalModData? instance)
    {
        instance = null;

        if (!Assert.IsValue(jsonElement, JsonValueKind.Object)) return false;

        if (!Assert.IsPropertyPresent(jsonElement, nameof(LocalModData.FileVersion), out var fileVersionProperty)) return false;
        
        var fileVersion = fileVersionProperty.GetUInt32();
        if (fileVersion != SUPPORTED_FILE_VERSION)
        {
            PluginLog.Warning($"Failed to read [{nameof(LocalModData)}], unsupported [{nameof(LocalModData.FileVersion)}] found [{fileVersion}] (supported version: {SUPPORTED_FILE_VERSION}): {jsonElement}");
            return false;
        }

        long? importDate = jsonElement.TryGetProperty(nameof(LocalModData.ImportDate), out var importDateProperty) ? importDateProperty.GetInt64() : null;

        var localTags = Array.Empty<string>();
        if (jsonElement.TryGetProperty(nameof(LocalModData.LocalTags), out var localTagsProperty) && !Assert.IsStringArray(localTagsProperty, out localTags))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(LocalModData.LocalTags)}] for [{nameof(LocalModData)}]: {localTagsProperty}");
            return false;
        }

        var favorite = jsonElement.TryGetProperty(nameof(LocalModData.Favorite), out var favoriteProperty) && favoriteProperty.GetBoolean();

        var preferredChangedItems = Array.Empty<int>();
        if (jsonElement.TryGetProperty(nameof(LocalModData.PreferredChangedItems), out var preferredChangedItemsProperty) && !Assert.IsIntArray(preferredChangedItemsProperty, out preferredChangedItems))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(LocalModData.PreferredChangedItems)}] for [{nameof(LocalModData)}]: {preferredChangedItems}");
            return false;
        }

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
