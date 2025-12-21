using ModOrganizer.Json.Readers.Penumbra.Manipulations;

namespace ModOrganizer.Json.Readers.Penumbra.Manipulations.Metas.Rsps;

// https://github.com/xivdev/Penumbra/blob/ce54aa5d2559abc8552edfe0b270e61c450226c4/schemas/structs/meta_rsp.json
public class MetaRsp : Manipulation
{
    public required float Entry { get; init; }
    public required string SubRace { get; init; }
    public required string Attribute { get; init; }
}
