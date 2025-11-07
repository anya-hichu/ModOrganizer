using ModOrganizer.Json.DefaultMods;
using ModOrganizer.Json.Groups;
using ModOrganizer.Json.LocalModData;
using ModOrganizer.Json.ModMetas;
using System.Collections.Generic;

namespace ModOrganizer.Mods;

public record ModInfo
{
    public required string Directory { get; init; }
    public required string Path { get; init; }

    public required Dictionary<string, object?> ChangedItems { get; init; }
    public required LocalModData? Data { get; init; }

    public required DefaultMod? Default { get; init; }
    public required List<Group?> Groups { get; init; }
    public required ModMeta? Meta { get; init; }
}
