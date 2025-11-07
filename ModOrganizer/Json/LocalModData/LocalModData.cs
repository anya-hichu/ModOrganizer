using System.Diagnostics;
using System.Linq;
using System.Text.Json;

namespace ModOrganizer.Json.LocalModData;

// https://github.com/xivdev/Penumbra/blob/318a41fe52ad00ce120d08b2c812e11a6a9b014a/schemas/local_mod_data-v3.json
public record LocalModData
{
    public required uint FileVersion { get; set; }
    public long? ImportDate { get; set; }
    public string[]? LocalTags { get; set; }
    public bool? Favorite { get; set; }
    public int[]? PreferredChangedItems { get; set; }
}
