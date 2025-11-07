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

    private ContainerBuilder<DefaultMod> ContainerBuilder { get; init; } = new(new(pluginLog), pluginLog);

    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out DefaultMod? instance)
    {
        instance = default;

        if (jsonElement.ValueKind != JsonValueKind.Object)
        {
            PluginLog.Warning($"Failed to build [{nameof(DefaultMod)}], expected root object but got [{jsonElement.ValueKind}]");
            return false;
        }

        uint? version = jsonElement.TryGetProperty(nameof(DefaultMod.Version), out var versionProperty) ? versionProperty.GetUInt32() : null;
        if (version != null && version != SUPPORTED_VERSION)
        {
            PluginLog.Warning($"Failed to build [{nameof(DefaultMod)}], unsupported [{nameof(DefaultMod.Version)}] [{version}] (supported version: {SUPPORTED_VERSION})");
            return false;
        }

        if (!ContainerBuilder.TryBuild(jsonElement, out var container))
        {
            PluginLog.Debug($"Failed to build [{nameof(DefaultMod)}]");
            return false;
        }

        instance = container with
        {
            Version = version
        };

        return true;
    }
}
