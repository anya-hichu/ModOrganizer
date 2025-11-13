namespace ModOrganizer.Json.ModMetas;

// https://github.com/xivdev/Penumbra/blob/318a41fe52ad00ce120d08b2c812e11a6a9b014a/schemas/mod_meta-v3.json

public record ModMeta
{
    public required uint FileVersion { get; init; }
    public required string Name { get; init; }

    public string? Author { get; init; }
    public string? Description { get; init; }
    public string? Image { get; init; }
    public string? Version { get; init; }
    public string? Website { get; init; }
    public string[]? ModTags { get; init; }
    public int[]? DefaultPreferredItems { get; init; }
    public string[]? RequiredFeatures { get; init; }
}
