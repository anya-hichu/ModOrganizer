namespace ModOrganizer.Json.Imcs;

public class ImcEntry
{
    public required byte MaterialId { get; set; }
    public required byte DecalId { get; set; }
    public required byte VfxId { get; set; }
    public required byte MaterialAnimationId { get; set; }
    public required ushort AttributeMask { get; set; }
    public required byte SoundId { get; set; }
}
