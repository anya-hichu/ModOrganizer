using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Manipulations.Wrapped;

public class MetaUnknownBuilder(IPluginLog pluginLog) : Builder<ManipulationWrapper>(pluginLog)
{
    public static readonly string TYPE = "Unknown";
    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out ManipulationWrapper? instance)
    {
        instance = new()
        {
            Type = TYPE,
            Manipulation = new MetaUnknown()
        };
        return true;
    }
}
