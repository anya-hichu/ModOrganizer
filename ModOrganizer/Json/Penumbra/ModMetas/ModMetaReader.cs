using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;

using ModOrganizer.Json.Readers.Elements;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.ModMetas;

public class ModMetaReader(IElementReader elementReader, IPluginLog pluginLog) : Reader<ModMeta>(pluginLog), IModMetaReader
{
    private static readonly uint SUPPORTED_FILE_VERSION = 3;

    public IElementReader ElementReader { get; init; } = elementReader;

    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out ModMeta? instance)
    {
        instance = null;

        if (!element.Is(JsonValueKind.Object, PluginLog)) return false;

        if (!element.TryGetRequiredPropertyValue(nameof(ModMeta.FileVersion), out uint fileVersion, PluginLog)) return false;

        if (fileVersion != SUPPORTED_FILE_VERSION)
        {
            PluginLog.Warning($"Failed to read [{nameof(ModMeta)}], unsupported [{nameof(ModMeta.FileVersion)}] found [{fileVersion}] (supported version: {SUPPORTED_FILE_VERSION}): {element}");
            return false;
        }

        if (!element.TryGetRequiredPropertyValue(nameof(ModMeta.Name), out string? name, PluginLog)) return false;

        if (!element.TryGetOptionalPropertyValue(nameof(ModMeta.Author), out string? author, PluginLog)) return false;
        if (!element.TryGetOptionalPropertyValue(nameof(ModMeta.Description), out string? description, PluginLog)) return false;
        if (!element.TryGetOptionalPropertyValue(nameof(ModMeta.Image), out string? image, PluginLog)) return false;
        if (!element.TryGetOptionalPropertyValue(nameof(ModMeta.Version), out string? version, PluginLog)) return false;
        if (!element.TryGetOptionalPropertyValue(nameof(ModMeta.Website), out string? website, PluginLog)) return false;

        if (!element.TryGetOptionalPropertyValue(nameof(ModMeta.ModTags), out string[]? modTags, PluginLog)) return false;
        if (!element.TryGetOptionalPropertyValue(nameof(ModMeta.DefaultPreferredItems), out int[]? defaultPreferredItems, PluginLog)) return false;
        if (!element.TryGetOptionalPropertyValue(nameof(ModMeta.RequiredFeatures), out string[]? requiredFeatures, PluginLog)) return false;

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
