using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Asserts;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Rsps;

public class MetaRspReader(IAssert assert, IPluginLog pluginLog) : Reader<MetaRsp>(assert, pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out MetaRsp? instance)
    {
        instance = null;

        if (!Assert.IsValue(element, JsonValueKind.Object)) return false;

        if (!Assert.IsPropertyPresent(element, nameof(MetaRsp.Entry), out var entryProperty)) return false;
        if (!Assert.IsValuePresent(element, nameof(MetaRsp.SubRace), out var subRace)) return false;
        if (!Assert.IsValuePresent(element, nameof(MetaRsp.Attribute), out var attribute)) return false;

        instance = new()
        {
            Entry = entryProperty.GetSingle(),
            SubRace = subRace,
            Attribute = attribute
        };

        return true;
    }
}
