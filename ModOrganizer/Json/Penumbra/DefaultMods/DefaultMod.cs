using ModOrganizer.Json.Penumbra.Containers;

namespace ModOrganizer.Json.Penumbra.DefaultMods;

// https://github.com/xivdev/Penumbra/blob/master/schemas/default_mod.json
public class DefaultMod : Container
{
    // unused currently
    public uint? Version { get; init; }
}
