using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Geqps;

public class MetaGeqpReader(IPluginLog pluginLog) : Reader<MetaGeqp>(pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out MetaGeqp? instance)
    {
        instance = null;

        if (!IsValue(element, JsonValueKind.Object)) return false;

        if (!TryGetOptionalU16Value(element, nameof(MetaGeqp.Condition), out var condition)) return false;
        if (!TryGetRequiredValue(element, nameof(MetaGeqp.Type), out var type)) return false;

        instance = new()
        {
            Condition = condition,
            Type = type
        };

        return true;
    }
}
