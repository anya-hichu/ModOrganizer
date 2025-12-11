namespace ModOrganizer.Json.Manipulations.Metas.Eqps;

// https://github.com/xivdev/Penumbra/blob/ce54aa5d2559abc8552edfe0b270e61c450226c4/schemas/structs/meta_eqp.json
public class MetaEqp : Manipulation
{
    public required ulong Entry { get; init; }
    public required ushort SetId { get; init; }
    public required string Slot { get; init; }
}
