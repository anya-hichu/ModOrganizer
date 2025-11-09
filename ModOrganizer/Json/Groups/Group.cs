namespace ModOrganizer.Json.Groups;

// https://github.com/xivdev/Penumbra/blob/318a41fe52ad00ce120d08b2c812e11a6a9b014a/schemas/group.json

public record Group
{
    // unused currently
    public uint? Version { get; set; }

    public required string Name { get; set; }
    public string? Description { get; set; }

    public string? Image { get; set; }

    public int? Page { get; set; }

    public int? Priority { get; set; }

    public required string Type { get; set; }

    public int? DefaultSettings { get; set; }
}
