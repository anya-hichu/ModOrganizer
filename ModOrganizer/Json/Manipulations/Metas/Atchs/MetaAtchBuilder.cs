using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Manipulations.Metas.Atchs;

public class MetaAtchBuilder(IPluginLog pluginLog) : Builder<MetaAtch>(pluginLog)
{
    private MetaAtchEntryBuilder MetaAtchEntryBuilder { get; init; } = new(pluginLog);

    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out MetaAtch? instance)
    {
        instance = null;

        if (!Assert.IsObject(jsonElement)) return false;

        if (!Assert.IsPropertyPresent(jsonElement, nameof(MetaAtch.Entry), out var entryProperty)) return false;
        if (!Assert.IsPropertyValuePresent(jsonElement, nameof(MetaAtch.Gender), out var gender)) return false;
        if (!Assert.IsPropertyValuePresent(jsonElement, nameof(MetaAtch.Race), out var race)) return false;
        if (!Assert.IsPropertyValuePresent(jsonElement, nameof(MetaAtch.Type), out var type)) return false;
        if (!Assert.IsU16PropertyValue(jsonElement, nameof(MetaAtch.Index), out var index)) return false;

        if (!MetaAtchEntryBuilder.TryBuild(entryProperty, out var entry))
        {
            PluginLog.Debug($"Failed to build [{nameof(MetaAtchEntry)}] for [{nameof(MetaAtch)}]:\n\t{entryProperty}");
            return false;
        }

        instance = new()
        {
            Entry = entry,
            Gender = gender,
            Race = race,
            Type = type,
            Index = index
        };

        return true;
    }
}
