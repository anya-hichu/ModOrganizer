using ModOrganizer.Json.Readers.Penumbra.Manipulations;

namespace ModOrganizer.Json.Readers.Penumbra.Manipulations.Metas.Atrs;

// https://github.com/xivdev/Penumbra/blob/318a41fe52ad00ce120d08b2c812e11a6a9b014a/schemas/structs/meta_atr.json
public class MetaAtr : Manipulation
{
    public bool? Entry { get; init; }
    public string? Slot { get; init; }
    public ushort? Id { get; init; }
    public required string Attribute { get; init; }
    public ushort? GenderRaceCondition { get; init; }
}
