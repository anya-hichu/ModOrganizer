using ModOrganizer.Json.Readers.Penumbra.Manipulations;

namespace ModOrganizer.Json.Readers.Penumbra.Manipulations.Metas.Gmps;

// https://github.com/xivdev/Penumbra/blob/ce54aa5d2559abc8552edfe0b270e61c450226c4/schemas/structs/meta_gmp.json
public class MetaGmp : Manipulation
{
    public required MetaGmpEntry Entry { get; init; }
    public required ushort SetId { get; init; } 
}
