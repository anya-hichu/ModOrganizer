using Dalamud.Plugin.Services;
using ModOrganizer.Json.Manipulations.Metas.Atch;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Manipulations.Metas;

public class MetaAtchBuilder(IPluginLog pluginLog) : Builder<MetaAtch>(pluginLog)
{
    private MetaAtchEntryBuilder MetaAtchEntryBuilder { get; init; } = new(pluginLog);

    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out MetaAtch? instance)
    {
        instance = default;

        if (!AssertIsObject(jsonElement)) return false;

        if (!AssertHasProperty(jsonElement, nameof(MetaAtch.Entry), out var entryProperty)) return false;
        if (!AssertStringPropertyPresent(jsonElement, nameof(MetaAtch.Gender), out var gender)) return false;
        if (!AssertStringPropertyPresent(jsonElement, nameof(MetaAtch.Race), out var race)) return false;
        if (!AssertStringPropertyPresent(jsonElement, nameof(MetaAtch.Type), out var type)) return false;
        if (!AssertHasProperty(jsonElement, nameof(MetaAtch.Index), out var indexProperty)) return false;

        if (!MetaAtchEntryBuilder.TryBuild(entryProperty, out var entry))
        {
            PluginLog.Debug($"Failed to build [{nameof(MetaAtchEntry)}] for [{nameof(MetaAtch)}]");
            return false;
        }

        instance = new()
        {
            Entry = entry,
            Gender = gender,
            Race = race,
            Type = type,
            Index = indexProperty.GetUInt16()
        };

        return true;
    }
}
