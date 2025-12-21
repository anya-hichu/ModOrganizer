using ModOrganizer.Json.Readers.Penumbra.Manipulations;

namespace ModOrganizer.Json.Readers.Penumbra.Manipulations.Metas.Atchs;

// https://github.com/xivdev/Penumbra/blob/318a41fe52ad00ce120d08b2c812e11a6a9b014a/schemas/structs/meta_atch.json
public class MetaAtch : Manipulation
{
    public required MetaAtchEntry Entry { get; init; }
    public required string Gender { get; init; }
    public required string Race { get; init; }
    public required string Type { get; init; }
    public required ushort Index { get; init; }
}
