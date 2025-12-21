namespace ModOrganizer.Json.Readers.Penumbra.Options;

// https://github.com/xivdev/Penumbra/blob/318a41fe52ad00ce120d08b2c812e11a6a9b014a/schemas/structs/option.json
public class Option : Data
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public int? Priority { get; init; }
    public string? Image { get; init; }
}
