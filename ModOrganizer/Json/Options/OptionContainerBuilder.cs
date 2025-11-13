using Dalamud.Plugin.Services;
using ModOrganizer.Json.Containers;
using ModOrganizer.Json.ModMetas;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Options;

public class OptionContainerBuilder(IPluginLog pluginLog) : Builder<OptionContainer>(pluginLog)
{
    private ContainerBuilder ContainerBuilder { get; init; } = new(pluginLog);
    private OptionBuilder OptionBuilder { get; init; } = new(pluginLog);

    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out OptionContainer? instance)
    {
        instance = null;

        if (!AssertObject(jsonElement)) return false;

        if (!ContainerBuilder.TryBuild(jsonElement, out var container))
        {
            PluginLog.Debug($"Failed to build base [{nameof(Container)}] for [{nameof(OptionContainer)}]");
            return false;
        }

        if (!OptionBuilder.TryBuild(jsonElement, out var option))
        {
            PluginLog.Debug($"Failed to build base [{nameof(Option)}] for [{nameof(OptionContainer)}]");
            return false;
        }

        instance = new()
        {
            Name = option.Name,
            Description = option.Description,
            Priority = option.Priority,
            Image = option.Image,

            Files = container.Files,
            FileSwaps = container.FileSwaps,
            Manipulations = container.Manipulations
        };

        return true;
    }
}
