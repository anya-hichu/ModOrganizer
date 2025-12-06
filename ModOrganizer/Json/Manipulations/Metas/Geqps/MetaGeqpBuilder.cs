using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Manipulations.Metas.Geqps;

public class MetaGeqpBuilder(IPluginLog pluginLog) : Builder<MetaGeqp>(pluginLog)
{
    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out MetaGeqp? instance)
    {
        instance = null;

        if (!Assert.IsValue(jsonElement, JsonValueKind.Object)) return false;

        Assert.IsU16Value(jsonElement, nameof(MetaGeqp.Condition), out var condition, false);
        if (!Assert.IsValuePresent(jsonElement, nameof(MetaGeqp.Type), out var type)) return false;

        instance = new()
        {
            Condition = condition,
            Type = type
        };

        return true;
    }
}
