namespace ModOrganizer.Json.Manipulations.Metas.Ests;

// https://github.com/xivdev/Penumbra/blob/ce54aa5d2559abc8552edfe0b270e61c450226c4/schemas/structs/meta_est.json
public record MetaEst : Manipulation
{
    public required ushort Entry { get; init; }
    public required string Gender { get; init; }
    public required string Race { get; init; }
    public required ushort SetId { get; init; }
    public required string Slot { get; init; }
}
