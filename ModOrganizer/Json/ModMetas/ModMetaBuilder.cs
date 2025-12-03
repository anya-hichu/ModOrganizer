using Dalamud.Plugin.Services;
using ModOrganizer.Json.Loaders;
using ModOrganizer.Json.Parsers;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;

namespace ModOrganizer.Json.ModMetas;

public class ModMetaBuilder(IPluginLog pluginLog) : Builder<ModMeta>(pluginLog), IFileParser<ModMeta>
{
    private static readonly int SUPPORTED_FILE_VERSION = 3;

    public JsonParser JsonParser { get; init; } = new(pluginLog);

    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out ModMeta? instance)
    {
        instance = null;

        if (!Assert.IsObject(jsonElement)) return false;

        if (!Assert.IsPropertyPresent(jsonElement, nameof(ModMeta.FileVersion), out var fileVersionProperty)) return false;

        var fileVersion = fileVersionProperty.GetUInt32();
        if (fileVersion != SUPPORTED_FILE_VERSION)
        {
            PluginLog.Warning($"Failed to build [{nameof(ModMeta)}], unsupported [{nameof(ModMeta.FileVersion)}] found [{fileVersion}] (supported version: {SUPPORTED_FILE_VERSION}):\n\t{jsonElement}");
            return false;
        }

        if (!Assert.IsPropertyValuePresent(jsonElement, nameof(ModMeta.Name), out var name)) return false;

        var author = jsonElement.TryGetProperty(nameof(ModMeta.Author), out var authorProperty) ? authorProperty.GetString() : null;
        var description = jsonElement.TryGetProperty(nameof(ModMeta.Description), out var descriptionProperty) ? descriptionProperty.GetString() : null;
        var image = jsonElement.TryGetProperty(nameof(ModMeta.Image), out var imageProperty) ? imageProperty.GetString() : null;
        var version = jsonElement.TryGetProperty(nameof(ModMeta.Version), out var versionProperty) ? versionProperty.GetString() : null;
        var website = jsonElement.TryGetProperty(nameof(ModMeta.Website), out var websiteProperty) ? websiteProperty.GetString() : null;

        // Provide default values to make it easier to use
        var modTags = jsonElement.TryGetProperty(nameof(ModMeta.ModTags), out var modTagsProperty) ? 
            modTagsProperty.EnumerateArray().Select(j => j.GetString()!).ToArray() : [];

        var defaultPreferredItems = jsonElement.TryGetProperty(nameof(ModMeta.DefaultPreferredItems), out var defaultPreferredItemsProperty) ? 
            defaultPreferredItemsProperty.EnumerateArray().Select(j => j.GetInt32()).ToArray() : [];

        var requiredFeatures = jsonElement.TryGetProperty(nameof(ModMeta.RequiredFeatures), out var requiredFeaturesProperty) ? 
            requiredFeaturesProperty.EnumerateArray().Select(j => j.GetString()!).ToArray() : [];

        instance = new()
        {
            FileVersion = fileVersion,
            Name = name,
            Author = author,
            Description = description,
            Image = image,
            Version = version,
            Website = website,
            ModTags = modTags,
            DefaultPreferredItems = defaultPreferredItems,
            RequiredFeatures = requiredFeatures,
        };

        return true;
    }
}
