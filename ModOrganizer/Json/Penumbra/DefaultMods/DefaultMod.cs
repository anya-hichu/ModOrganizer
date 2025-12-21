using ModOrganizer.Json.Readers.Penumbra.Containers;

namespace ModOrganizer.Json.Readers.Penumbra.DefaultMods;

// https://github.com/xivdev/Penumbra/blob/master/schemas/default_mod.json
public class DefaultMod : Container
{
    // unused currently
    public uint? Version { get; init; }
}
