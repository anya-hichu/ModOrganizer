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

        if (!IsValue(element, JsonValueKind.Object)) return false;

        if (!TryGetRequiredProperty(element, nameof(ModMeta.FileVersion), out var fileVersionProperty)) return false;

        var fileVersion = fileVersionProperty.GetUInt32();
        if (fileVersion != SUPPORTED_FILE_VERSION)
        {
            PluginLog.Warning($"Failed to read [{nameof(ModMeta)}], unsupported [{nameof(ModMeta.FileVersion)}] found [{fileVersion}] (supported version: {SUPPORTED_FILE_VERSION}): {element}");
            return false;
        }

        if (!TryGetRequiredValue(element, nameof(ModMeta.Name), out var name)) return false;

        if (!TryGetOptionalValue(element, nameof(ModMeta.Author), out string? author)) return false;
        if (!TryGetOptionalValue(element, nameof(ModMeta.Description), out string? description)) return false;
        if (!TryGetOptionalValue(element, nameof(ModMeta.Image), out string? image)) return false;
        if (!TryGetOptionalValue(element, nameof(ModMeta.Version), out string? version)) return false;
        if (!TryGetOptionalValue(element, nameof(ModMeta.Website), out string? website)) return false;

        if (!TryGetOptionalArrayValue(element, nameof(ModMeta.ModTags), out string[]? modTags)) return false;
        if (!TryGetOptionalArrayValue(element, nameof(ModMeta.DefaultPreferredItems), out int[]? defaultPreferredItems)) return false;
        if (!TryGetOptionalArrayValue(element, nameof(ModMeta.RequiredFeatures), out string[]? requiredFeatures)) return false;

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
