namespace ModOrganizer.Json.Readers.Penumbra.Groups;

// https://github.com/xivdev/Penumbra/blob/318a41fe52ad00ce120d08b2c812e11a6a9b014a/schemas/group.json

public class Group : Data
{
    // unused currently
    public uint? Version { get; init; }

    public required string Name { get; init; }
    public string? Description { get; init; }

    public string? Image { get; init; }

    public int? Page { get; init; }

    public int? Priority { get; init; }

    public required string Type { get; init; }

    public int? DefaultSettings { get; init; }
}
