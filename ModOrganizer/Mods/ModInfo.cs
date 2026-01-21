using ModOrganizer.Json.Penumbra.DefaultMods;
using ModOrganizer.Json.Penumbra.Groups;
using ModOrganizer.Json.Penumbra.LocalModDatas;
using ModOrganizer.Json.Penumbra.ModMetas;
using System.Collections.Generic;

namespace ModOrganizer.Mods;

public class ModInfo
{
    public required string Directory { get; init; }
    public required string Path { get; init; }

    public required Dictionary<string, object?> ChangedItems { get; init; }
    public required LocalModDataV3 Data { get; init; }

    public required DefaultMod Default { get; init; }
    public required ModMetaV3 Meta { get; init; }
    public required Group[] Groups { get; init; }
}
