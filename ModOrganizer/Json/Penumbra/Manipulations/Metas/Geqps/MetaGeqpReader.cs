using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Elements;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Geqps;

public class MetaGeqpReader(IPluginLog pluginLog) : Reader<MetaGeqp>(pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out MetaGeqp? instance)
    {
        instance = null;

        if (!element.Is(JsonValueKind.Object, PluginLog)) return false;

        if (!element.TryGetOptionalPropertyU16Value(nameof(MetaGeqp.Condition), out var condition, PluginLog)) return false;
        if (!element.TryGetRequiredNotEmptyPropertyValue(nameof(MetaGeqp.Type), out var type, PluginLog)) return false;

        instance = new()
        {
            Condition = condition,
            Type = type
        };

        return true;
    }
}
