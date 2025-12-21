using ModOrganizer.Json.Readers.Penumbra.DefaultMods;
using ModOrganizer.Json.Readers.Penumbra.Groups;
using ModOrganizer.Json.Readers.Penumbra.LocalModDatas;
using ModOrganizer.Json.Readers.Penumbra.ModMetas;
using System.Collections.Generic;

namespace ModOrganizer.Mods;

public class ModInfo
{
    public required string Directory { get; init; }
    public required string Path { get; init; }

    public required Dictionary<string, object?> ChangedItems { get; init; }
    public required LocalModData Data { get; init; }

    public required DefaultMod Default { get; init; }
    public required ModMeta Meta { get; init; }
    public required Group[] Groups { get; init; }
}
