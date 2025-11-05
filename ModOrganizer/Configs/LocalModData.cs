using System.Numerics;

namespace ModOrganizer.Configs;

// https://github.com/xivdev/Penumbra/blob/master/schemas/local_mod_data-v3.json
public class LocalModData
{
    public int FileVersion { get; set; }
    public long? ImportDate { get; set; }
    public string[]? LocalTags { get; set; } = [];
    public bool? Favorite { get; set; }
    public int[]? PreferredChangedItems { get; set; } = [];
}
