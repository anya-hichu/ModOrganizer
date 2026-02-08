using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;

using ModOrganizer.Json.Readers.Elements;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.ModMetas;

public class ModMetaReader(IElementReader elementReader, IPluginLog pluginLog) : Reader<ModMetaV3>(pluginLog), IModMetaReader
{
    public static readonly uint SUPPORTED_FILE_VERSION = 3;

    public IElementReader ElementReader { get; init; } = elementReader;

    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out ModMetaV3? instance)
    {
        instance = null;

        if (!element.Is(JsonValueKind.Object, PluginLog)) return false;

        if (!element.TryGetRequiredPropertyValue(nameof(ModMetaV3.FileVersion), out uint fileVersion, PluginLog)) return false;

        if (fileVersion != SUPPORTED_FILE_VERSION)
        {
            PluginLog.Warning($"Failed to read [{nameof(ModMetaV3)}], unsupported [{nameof(ModMetaV3.FileVersion)}] found [{fileVersion}] (supported version: {SUPPORTED_FILE_VERSION}): {element}");
            return false;
        }

        if (!element.TryGetRequiredPropertyValue(nameof(ModMetaV3.Name), out string? name, PluginLog)) return false;

        if (!element.TryGetOptionalPropertyValue(nameof(ModMetaV3.Author), out string? author, PluginLog)) return false;
        if (!element.TryGetOptionalPropertyValue(nameof(ModMetaV3.Description), out string? description, PluginLog)) return false;
        if (!element.TryGetOptionalPropertyValue(nameof(ModMetaV3.Image), out string? image, PluginLog)) return false;
        if (!element.TryGetOptionalPropertyValue(nameof(ModMetaV3.Version), out string? version, PluginLog)) return false;
        if (!element.TryGetOptionalPropertyValue(nameof(ModMetaV3.Website), out string? website, PluginLog)) return false;

        if (!element.TryGetOptionalPropertyValue(nameof(ModMetaV3.ModTags), out string[]? modTags, PluginLog)) return false;
        if (!element.TryGetOptionalPropertyValue(nameof(ModMetaV3.DefaultPreferredItems), out int[]? defaultPreferredItems, PluginLog)) return false;
        if (!element.TryGetOptionalPropertyValue(nameof(ModMetaV3.RequiredFeatures), out string[]? requiredFeatures, PluginLog)) return false;

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
