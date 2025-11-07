namespace ModOrganizer.Json.Options;

// https://github.com/xivdev/Penumbra/blob/318a41fe52ad00ce120d08b2c812e11a6a9b014a/schemas/structs/option.json
public record Option
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int? Priority { get; set; }
    public string? Image { get; set; }
}
