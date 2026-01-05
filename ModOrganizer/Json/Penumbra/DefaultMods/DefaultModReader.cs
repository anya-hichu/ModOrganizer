using Dalamud.Plugin.Services;
using ModOrganizer.Json.Penumbra.Containers;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Asserts;
using ModOrganizer.Json.Readers.Elements;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.DefaultMods;

public class DefaultModReader(IAssert assert, IReader<Container> containerReader, IElementReader elementReader, IPluginLog pluginLog) : Reader<DefaultMod>(assert, pluginLog), IDefaultModReader
{
    private static readonly uint SUPPORTED_VERSION = 0;

    public IElementReader ElementReader { get; init; } = elementReader;

    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out DefaultMod? instance)
    {
        instance = null;

        if (!Assert.IsValue(element, JsonValueKind.Object)) return false;

        uint? version = element.TryGetProperty(nameof(DefaultMod.Version), out var versionProperty) ? versionProperty.GetUInt32() : null;
        if (version != null && version != SUPPORTED_VERSION)
        {
            PluginLog.Warning($"Failed to read [{nameof(DefaultMod)}], unsupported [{nameof(DefaultMod.Version)}] found [{version}] (supported version: [{SUPPORTED_VERSION}]): {element}");
            return false;
        }

        if (!containerReader.TryRead(element, out var container))
        {
            PluginLog.Debug($"Failed to read base [{nameof(Container)}] for [{nameof(DefaultMod)}]: {element}");
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
