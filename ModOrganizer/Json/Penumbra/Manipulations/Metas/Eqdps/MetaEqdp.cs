using ModOrganizer.Json.Penumbra.Manipulations;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Eqdps;

// https://github.com/xivdev/Penumbra/blob/ce54aa5d2559abc8552edfe0b270e61c450226c4/schemas/structs/meta_eqdp.json
public class MetaEqdp : Manipulation
{
    public required ushort Entry { get; init; }
    public required string Gender { get; init; }
    public required string Race { get; init; }
    public required ushort SetId { get; init; }
    public required string Slot { get; init; }
}
