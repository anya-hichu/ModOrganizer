namespace ModOrganizer.Json.Options.Imcs;

public record OptionImcAttributeMask : OptionImc
{
    public required ushort AttributeMask { get; init; }
}
