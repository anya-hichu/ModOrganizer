namespace ModOrganizer.Json.Manipulations.Metas.Atch;

public record MetaAtchEntry
{
    public required string Bone { get; set; }
    public required float Scale { get; set; }
    public required float OffsetX { get; set; }
    public required float OffsetY { get; set; }
    public required float OffsetZ { get; set; }
    public required float RotationX { get; set; }
    public required float RotationY { get; set; }
    public required float RotationZ { get; set; }
}
