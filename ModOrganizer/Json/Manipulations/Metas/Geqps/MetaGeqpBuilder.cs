using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Manipulations.Metas.Geqps;

public class MetaGeqpBuilder(IPluginLog pluginLog) : Builder<MetaGeqp>(pluginLog)
{
    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out MetaGeqp? instance)
    {
        instance = null;

        if (!Assert.IsObject(jsonElement)) return false;

        Assert.IsU16PropertyValue(jsonElement, nameof(MetaGeqp.Condition), out var condition, false);
        if (!Assert.IsPropertyValuePresent(jsonElement, nameof(MetaGeqp.Type), out var type)) return false;

        instance = new()
        {
            Condition = condition,
            Type = type
        };

        return true;
    }
}
