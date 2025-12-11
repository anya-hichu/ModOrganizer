using Dalamud.Plugin.Services;
using ModOrganizer.Json.Files;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.SortOrders;

public class SortOrderBuilder(IPluginLog pluginLog) : Builder<SortOrder>(pluginLog), IFileBuilder<SortOrder>
{
    public FileParser FileParser { get; init; } = new(pluginLog);

    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out SortOrder? instance)
    {
        instance = null;

        if (!Assert.IsValue(jsonElement, JsonValueKind.Object)) return false;
        if (!Assert.IsPropertyPresent(jsonElement, nameof(SortOrder.Data), out var dataProperty)) return false;

        if (!Assert.IsStringDict(dataProperty, out var data))
        {
            PluginLog.Warning($"Failed to build one or more [{nameof(SortOrder.Data)}] for [{nameof(SortOrder)}]:\n\t{dataProperty}");
            return false;
        }

        instance = new() { Data = data };
        return true;
    }
}
