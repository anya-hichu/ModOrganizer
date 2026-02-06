namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Atchs.Entries;

public class MetaAtchEntry : Data
{
    public required string Bone { get; init; }
    public required float Scale { get; init; }
    public required float OffsetX { get; init; }
    public required float OffsetY { get; init; }
    public required float OffsetZ { get; init; }
    public required float RotationX { get; init; }
    public required float RotationY { get; init; }
    public required float RotationZ { get; init; }
}
