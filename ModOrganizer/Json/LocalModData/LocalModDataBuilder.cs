using Dalamud.Plugin.Services;
using ModOrganizer.Json.Files;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;

namespace ModOrganizer.Json.LocalModData;

public class LocalModDataBuilder(IPluginLog pluginLog) : Builder<LocalModData>(pluginLog), IFileBuilder<LocalModData>
{
    private static readonly int SUPPORTED_FILE_VERSION = 3;

    public FileParser FileParser { get; init; } = new(pluginLog);

    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out LocalModData? instance)
    {
        instance = null;

        if (!Assert.IsObject(jsonElement)) return false;

        if (!Assert.IsPropertyPresent(jsonElement, nameof(LocalModData.FileVersion), out var fileVersionProperty)) return false;
        
        var fileVersion = fileVersionProperty.GetUInt32();
        if (fileVersion != SUPPORTED_FILE_VERSION)
        {
            PluginLog.Warning($"Failed to build [{nameof(LocalModData)}], unsupported [{nameof(LocalModData.FileVersion)}] found [{fileVersion}] (supported version: {SUPPORTED_FILE_VERSION}):\n\t{jsonElement}");
            return false;
        }

        long? importDate = jsonElement.TryGetProperty(nameof(LocalModData.ImportDate), out var importDateProperty) ? importDateProperty.GetInt64() : null;

        // Provide default values to make it easier to use
        var localTags = jsonElement.TryGetProperty(nameof(LocalModData.LocalTags), out var localTagsProperty) ? 
            localTagsProperty.EnumerateArray().Select(j => j.GetString()!).ToArray() : [];

        var favorite = jsonElement.TryGetProperty(nameof(LocalModData.Favorite), out var favoriteProperty) && favoriteProperty.GetBoolean();

        var preferredChangedItems = jsonElement.TryGetProperty(nameof(LocalModData.PreferredChangedItems), out var preferredChangedItemsProperty) ? 
            preferredChangedItemsProperty.EnumerateArray().Select(j => j.GetInt32()).ToArray() : [];

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
