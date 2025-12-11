namespace ModOrganizer.Json.Manipulations.Metas.Eqdps;

// https://github.com/xivdev/Penumbra/blob/ce54aa5d2559abc8552edfe0b270e61c450226c4/schemas/structs/meta_shp.json
public class MetaShp : Manipulation
{
    public bool? Entry { get; init; }
    public string? Slot { get; init; }
    public ushort? Id { get; init; }
    public required string Shape { get; init; }
    public string? ConnectorCondition { get; init; }
    public ushort? GenderRaceCondition { get; init; }
}
