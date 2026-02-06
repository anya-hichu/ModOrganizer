namespace ModOrganizer.Json.Penumbra.Manipulations.Wrappers;

// https://github.com/xivdev/Penumbra/blob/318a41fe52ad00ce120d08b2c812e11a6a9b014a/schemas/structs/manipulation.json
public class ManipulationWrapper
{
    public required string Type { get; init; }
    public required Manipulation Manipulation { get; init; }
}
