using ModOrganizer.Json.Manipulations;

namespace ModOrganizer.Json.Manipulations.Metas.Imcs;

// https://github.com/xivdev/Penumbra/blob/318a41fe52ad00ce120d08b2c812e11a6a9b014a/schemas/structs/meta_imc.json
public record MetaImcIdentifier : Manipulation
{
    public required ushort PrimaryId { get; set; }
    public required ushort SecondaryId { get; set; }
    public required byte Variant { get; set; }
    public required string ObjectType { get; set; }
    public required string EquipSlot { get; set; }
    public required string BodySlot { get; set; }
}
