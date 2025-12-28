using ModOrganizer.Json.Penumbra.Manipulations;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Geqps;

// https://github.com/xivdev/Penumbra/blob/ce54aa5d2559abc8552edfe0b270e61c450226c4/schemas/structs/meta_geqp.json
public class MetaGeqp : Manipulation
{
    public ushort? Condition { get; init; }
    public required string Type { get; init; }
}
