using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Containers;

public class NamedContainerBuilder(IPluginLog pluginLog) : Builder<NamedContainer>(pluginLog)
{
    private ContainerBuilder ContainerBuilder { get; set; } = new(pluginLog);

    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out NamedContainer? instance)
    {
        instance = default;

        if(!ContainerBuilder.TryBuild(jsonElement, out var container))
        {
            PluginLog.Debug($"Failed to build base [{nameof(Container)}] for [{nameof(NamedContainer)}]");
            return false;
        }

        var name = jsonElement.TryGetProperty(nameof(NamedContainer.Name), out var nameProperty) ? nameProperty.GetString() : null;

        instance = new()
        {
            Name = name,
            Files = container.Files,
            FileSwaps = container.FileSwaps,
            Manipulations = container.Manipulations
        };

        return true;
    }
}
