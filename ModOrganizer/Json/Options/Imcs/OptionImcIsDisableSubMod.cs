namespace ModOrganizer.Json.Options.Imcs;

public record OptionImcIsDisableSubMod : OptionImc
{
    public required bool IsDisableSubMod { get; set; }
}
