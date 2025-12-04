using Dalamud.Plugin.Services;
using ModOrganizer.Json.Files;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;

namespace ModOrganizer.Json.SortOrders;

public class SortOrderBuilder(IPluginLog pluginLog) : Builder<SortOrder>(pluginLog), IFileBuilder<SortOrder>
{
    public FileParser FileParser { get; init; } = new(pluginLog);

    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out SortOrder? instance)
    {
        instance = null;

        if (!Assert.IsObject(jsonElement)) return false;

        if (!Assert.IsPropertyPresent(jsonElement, nameof(SortOrder.Data), out var dataProperty)) return false;

        var data = dataProperty.EnumerateObject().ToDictionary(p => p.Name, p => p.Value.GetString()!);

        instance = new()
        {
            Data = data
        };

        return true;
    }
}
