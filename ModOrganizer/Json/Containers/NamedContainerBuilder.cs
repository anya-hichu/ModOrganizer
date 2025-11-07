using Dalamud.Plugin.Services;
using ModOrganizer.Json.Manipulations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Containers;

public class NamedContainerBuilder(ManipulationFactory manipulationFactory, IPluginLog pluginLog) : ContainerBuilder<NamedContainer>(manipulationFactory, pluginLog)
{
    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out NamedContainer? instance)
    {
        instance = default;

        if(!base.TryBuild(jsonElement, out var container))
        {
            PluginLog.Debug($"Failed to build [{nameof(NamedContainer)}]");
            return false;
        }

        var name = jsonElement.TryGetProperty(nameof(NamedContainer.Name), out var nameProperty) ? nameProperty.GetString() : null;

        instance = container with
        {
            Name = name
        };

        return true;
    }
}
