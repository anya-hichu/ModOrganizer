using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;

using ModOrganizer.Json.Readers.Elements;
using System;
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

        if (!IsValue(element, JsonValueKind.Object)) return false;

        if (!TryGetRequiredProperty(element, nameof(LocalModData.FileVersion), out var fileVersionProperty)) return false;
        
        var fileVersion = fileVersionProperty.GetUInt32();
        if (fileVersion != SUPPORTED_FILE_VERSION)
        {
            PluginLog.Warning($"Failed to read [{nameof(LocalModData)}], unsupported [{nameof(LocalModData.FileVersion)}] found [{fileVersion}] (supported version: {SUPPORTED_FILE_VERSION}): {element}");
            return false;
        }

        long? importDate = element.TryGetProperty(nameof(LocalModData.ImportDate), out var importDateProperty) ? importDateProperty.GetInt64() : null;

        if (!TryGetOptionalArrayValue(element, nameof(LocalModData.LocalTags), out string[]? localTags))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(LocalModData.LocalTags)}] for [{nameof(LocalModData)}]: {element}");
            return false;
        }

        var favorite = element.TryGetProperty(nameof(LocalModData.Favorite), out var favoriteProperty) && favoriteProperty.GetBoolean();

        if (!TryGetOptionalArrayValue(element, nameof(LocalModData.PreferredChangedItems), out int[]? preferredChangedItems))
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
