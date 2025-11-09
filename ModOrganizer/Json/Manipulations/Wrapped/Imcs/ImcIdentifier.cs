namespace ModOrganizer.Json.Manipulations.Wrapped.Imcs;

public record ImcIdentifier : Manipulation
{
    public required ushort PrimaryId { get; set; }
    public required ushort SecondaryId { get; set; }
    public required byte Variant { get; set; }
    public required string ObjectType { get; set; }
    public required string EquipSlot { get; set; }
    public required string BodySlot { get; set; }
}
