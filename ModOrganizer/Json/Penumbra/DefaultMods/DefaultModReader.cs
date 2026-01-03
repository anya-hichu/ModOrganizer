using Dalamud.Plugin.Services;
using ModOrganizer.Json.Penumbra.Containers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Elements;

namespace ModOrganizer.Json.Penumbra.DefaultMods;

public class DefaultModReader(IReader<Container> containerReader, IElementReader elementReader, IPluginLog pluginLog) : Reader<DefaultMod>(pluginLog), IDefaultModReader
{
    private static readonly uint SUPPORTED_VERSION = 0;

    public IElementReader ElementReader { get; init; } = elementReader;

    public override bool TryRead(JsonElement jsonElement, [NotNullWhen(true)] out DefaultMod? instance)
    {
        instance = null;

        if (!Assert.IsValue(jsonElement, JsonValueKind.Object)) return false;

        uint? version = jsonElement.TryGetProperty(nameof(DefaultMod.Version), out var versionProperty) ? versionProperty.GetUInt32() : null;
        if (version != null && version != SUPPORTED_VERSION)
        {
            PluginLog.Warning($"Failed to read [{nameof(DefaultMod)}], unsupported [{nameof(DefaultMod.Version)}] found [{version}] (supported version: [{SUPPORTED_VERSION}]): {jsonElement}");
            return false;
        }

        if (!containerReader.TryRead(jsonElement, out var container))
        {
            PluginLog.Debug($"Failed to read base [{nameof(Container)}] for [{nameof(DefaultMod)}]: {jsonElement}");
            return false;
        }

        instance = new()
        {
            Version = version,
            Files = container.Files,
            FileSwaps = container.FileSwaps,
            Manipulations = container.Manipulations
        };

        return true;
    }
}
