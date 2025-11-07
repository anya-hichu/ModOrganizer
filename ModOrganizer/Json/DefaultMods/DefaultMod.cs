using ModOrganizer.Json.Containers;

namespace ModOrganizer.Json.DefaultMods;

// https://github.com/xivdev/Penumbra/blob/master/schemas/default_mod.json
public record DefaultMod : Container
{
    // unused currently
    public uint? Version { get; set;}
}
