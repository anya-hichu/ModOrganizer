using Dalamud.Plugin.Services;
using ModOrganizer.Json.Containers;
using ModOrganizer.Json.Loaders;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.DefaultMods;

public class DefaultModBuilder(IPluginLog pluginLog) : Builder<DefaultMod>(pluginLog), IFileLoader<DefaultMod>
{
    private static readonly int SUPPORTED_VERSION = 0;

    public JsonParser JsonParser { get; init; } = new(pluginLog);

    private ContainerBuilder ContainerBuilder { get; init; } = new(pluginLog);

    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out DefaultMod? instance)
    {
        instance = null;

        if (!Assert.IsObject(jsonElement)) return false;

        uint? version = jsonElement.TryGetProperty(nameof(DefaultMod.Version), out var versionProperty) ? versionProperty.GetUInt32() : null;
        if (version != null && version != SUPPORTED_VERSION)
        {
            PluginLog.Warning($"Failed to build [{nameof(DefaultMod)}], unsupported [{nameof(DefaultMod.Version)}] found [{version}] (supported version: {SUPPORTED_VERSION}):\n\t{jsonElement}");
            return false;
        }

        if (!ContainerBuilder.TryBuild(jsonElement, out var container))
        {
            PluginLog.Debug($"Failed to build base [{nameof(Container)}] for [{nameof(DefaultMod)}]:\n\t{jsonElement}");
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
