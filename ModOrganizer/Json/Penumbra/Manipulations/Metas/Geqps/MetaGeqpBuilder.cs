using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Asserts;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Geqps;

public class MetaGeqpReader(IAssert assert, IPluginLog pluginLog) : Reader<MetaGeqp>(assert, pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out MetaGeqp? instance)
    {
        instance = null;

        if (!Assert.IsValue(element, JsonValueKind.Object)) return false;

        Assert.IsU16Value(element, nameof(MetaGeqp.Condition), out var condition, false);
        if (!Assert.IsValuePresent(element, nameof(MetaGeqp.Type), out var type)) return false;

        instance = new()
        {
            Condition = condition,
            Type = type
        };

        return true;
    }
}
