namespace ModOrganizer.Json.ModMetas;

// https://github.com/xivdev/Penumbra/blob/318a41fe52ad00ce120d08b2c812e11a6a9b014a/schemas/mod_meta-v3.json

public record ModMeta
{
    public required uint FileVersion { get; set; }
    public required string Name { get; set; }

    public string? Author { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
    public string? Version { get; set; }
    public string? Website { get; set; }
    public string[]? ModTags { get; set; }
    public int[]? DefaultPreferredItems { get; set; }
    public string[]? RequiredFeatures { get; set; }
}
