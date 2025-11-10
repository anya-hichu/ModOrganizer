using ModOrganizer.Json.Manipulations.Metas.Atch;

namespace ModOrganizer.Json.Manipulations.Metas;

// https://github.com/xivdev/Penumbra/blob/318a41fe52ad00ce120d08b2c812e11a6a9b014a/schemas/structs/meta_atch.json
public record MetaAtch : Manipulation
{
    public required MetaAtchEntry Entry { get; set; }
    public required string Gender { get; set; }
    public required string Race { get; set; }
    public required string Type { get; set; }
    public required ushort Index { get; set; }
}
