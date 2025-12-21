using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers.Files;
using ModOrganizer.Json.Readers.Penumbra.Containers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Readers.Penumbra.DefaultMods;

public class DefaultModReader(IPluginLog pluginLog) : Reader<DefaultMod>(pluginLog), IReadableFile<DefaultMod>
{
    private static readonly int SUPPORTED_VERSION = 0;

    public FileReader FileReader { get; init; } = new(pluginLog);

    private ContainerReader ContainerReader { get; init; } = new(pluginLog);

    public override bool TryRead(JsonElement jsonElement, [NotNullWhen(true)] out DefaultMod? instance)
    {
        instance = null;

        if (!Assert.IsValue(jsonElement, JsonValueKind.Object)) return false;

        uint? version = jsonElement.TryGetProperty(nameof(DefaultMod.Version), out var versionProperty) ? versionProperty.GetUInt32() : null;
        if (version != null && version != SUPPORTED_VERSION)
        {
            PluginLog.Warning($"Failed to read [{nameof(DefaultMod)}], unsupported [{nameof(DefaultMod.Version)}] found [{version}] (supported version: [{SUPPORTED_VERSION}]):\n\t{jsonElement}");
            return false;
        }

        if (!ContainerReader.TryRead(jsonElement, out var container))
        {
            PluginLog.Debug($"Failed to read base [{nameof(Container)}] for [{nameof(DefaultMod)}]:\n\t{jsonElement}");
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
