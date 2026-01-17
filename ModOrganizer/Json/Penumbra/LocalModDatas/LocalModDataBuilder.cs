using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Asserts;
using ModOrganizer.Json.Readers.Elements;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.LocalModDatas;

public class LocalModDataReader(IAssert assert, IElementReader elementReader, IPluginLog pluginLog) : Reader<LocalModData>(assert, pluginLog), ILocalModDataReader
{
    public static readonly uint SUPPORTED_FILE_VERSION = 3;

    public IElementReader ElementReader { get; init; } = elementReader;

    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out LocalModData? instance)
    {
        instance = null;

        if (!Assert.IsValue(element, JsonValueKind.Object)) return false;

        if (!Assert.IsPropertyPresent(element, nameof(LocalModData.FileVersion), out var fileVersionProperty)) return false;
        
        var fileVersion = fileVersionProperty.GetUInt32();
        if (fileVersion != SUPPORTED_FILE_VERSION)
        {
            PluginLog.Warning($"Failed to read [{nameof(LocalModData)}], unsupported [{nameof(LocalModData.FileVersion)}] found [{fileVersion}] (supported version: {SUPPORTED_FILE_VERSION}): {element}");
            return false;
        }

        long? importDate = element.TryGetProperty(nameof(LocalModData.ImportDate), out var importDateProperty) ? importDateProperty.GetInt64() : null;

        var localTags = Array.Empty<string>();
        if (element.TryGetProperty(nameof(LocalModData.LocalTags), out var localTagsProperty) && !Assert.IsStringArray(localTagsProperty, out localTags))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(LocalModData.LocalTags)}] for [{nameof(LocalModData)}]: {element}");
            return false;
        }

        var favorite = element.TryGetProperty(nameof(LocalModData.Favorite), out var favoriteProperty) && favoriteProperty.GetBoolean();

        var preferredChangedItems = Array.Empty<int>();
        if (element.TryGetProperty(nameof(LocalModData.PreferredChangedItems), out var preferredChangedItemsProperty) && !Assert.IsIntArray(preferredChangedItemsProperty, out preferredChangedItems))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(LocalModData.PreferredChangedItems)}] for [{nameof(LocalModData)}]: {element}");
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
