using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Asserts;
using ModOrganizer.Json.Readers.Elements;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.ModMetas;

public class ModMetaReader(IAssert assert, IElementReader elementReader, IPluginLog pluginLog) : Reader<ModMeta>(assert, pluginLog), IModMetaReader
{
    private static readonly uint SUPPORTED_FILE_VERSION = 3;

    public IElementReader ElementReader { get; init; } = elementReader;

    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out ModMeta? instance)
    {
        instance = null;

        if (!Assert.IsValue(element, JsonValueKind.Object)) return false;

        if (!Assert.IsPropertyPresent(element, nameof(ModMeta.FileVersion), out var fileVersionProperty)) return false;

        var fileVersion = fileVersionProperty.GetUInt32();
        if (fileVersion != SUPPORTED_FILE_VERSION)
        {
            PluginLog.Warning($"Failed to read [{nameof(ModMeta)}], unsupported [{nameof(ModMeta.FileVersion)}] found [{fileVersion}] (supported version: {SUPPORTED_FILE_VERSION}): {element}");
            return false;
        }

        if (!Assert.IsValuePresent(element, nameof(ModMeta.Name), out var name)) return false;

        var author = element.TryGetProperty(nameof(ModMeta.Author), out var authorProperty) ? authorProperty.GetString() : null;
        var description = element.TryGetProperty(nameof(ModMeta.Description), out var descriptionProperty) ? descriptionProperty.GetString() : null;
        var image = element.TryGetProperty(nameof(ModMeta.Image), out var imageProperty) ? imageProperty.GetString() : null;
        var version = element.TryGetProperty(nameof(ModMeta.Version), out var versionProperty) ? versionProperty.GetString() : null;
        var website = element.TryGetProperty(nameof(ModMeta.Website), out var websiteProperty) ? websiteProperty.GetString() : null;

        var modTags = Array.Empty<string>();
        if (element.TryGetProperty(nameof(ModMeta.ModTags), out var modTagsProperty) && !Assert.IsStringArray(modTagsProperty, out modTags))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(ModMeta.ModTags)}] for [{nameof(ModMeta)}]: {element}");
            return false;
        }

        var defaultPreferredItems = Array.Empty<int>();
        if (element.TryGetProperty(nameof(ModMeta.DefaultPreferredItems), out var defaultPreferredItemsProperty) && !Assert.IsIntArray(defaultPreferredItemsProperty, out defaultPreferredItems))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(ModMeta.DefaultPreferredItems)}] for [{nameof(ModMeta)}]: {element}");
            return false;
        }

        var requiredFeatures = Array.Empty<string>();
        if (element.TryGetProperty(nameof(ModMeta.RequiredFeatures), out var requiredFeaturesProperty) && !Assert.IsStringArray(requiredFeaturesProperty, out requiredFeatures))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(ModMeta.RequiredFeatures)}] for [{nameof(ModMeta)}]: {element}");
            return false;
        }

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
            RequiredFeatures = requiredFeatures
        };

        return true;
    }
}
