namespace ModOrganizer.Json.Manipulations.Metas.Gmps;

public class MetaGmpEntry : Data
{
    public required bool Enabled { get; init; }
    public required bool Animated { get; init; }
    public required ushort RotationA { get; init; }
    public required ushort RotationB { get; init; }
    public required ushort RotationC { get; init; }
    public required byte UnknownA { get; init; }
    public required byte UnknownB { get; init; }
}
