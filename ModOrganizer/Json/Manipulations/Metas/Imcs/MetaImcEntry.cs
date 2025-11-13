namespace ModOrganizer.Json.Manipulations.Metas.Imcs;

// https://github.com/xivdev/Penumbra/blob/318a41fe52ad00ce120d08b2c812e11a6a9b014a/schemas/structs/meta_imc.json
public record MetaImcEntry
{
    public required byte MaterialId { get; set; }
    public required byte DecalId { get; set; }
    public required byte VfxId { get; set; }
    public required byte MaterialAnimationId { get; set; }
    public required ushort AttributeMask { get; set; }
    public required byte SoundId { get; set; }
}
