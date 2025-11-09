using ModOrganizer.Json.Imcs;
using ModOrganizer.Json.Options.Imcs;

namespace ModOrganizer.Json.Groups;

// https://github.com/xivdev/Penumbra/blob/318a41fe52ad00ce120d08b2c812e11a6a9b014a/schemas/structs/group_imc.json
public record GroupImc : Group
{
    public bool? AllVariants { get; set; }
    public bool? AllAttributes { get; set; }
    public ImcIdentifier? Identifier { get; set; }
    public ImcEntry? DefaultEntry { get; set; }
    public OptionImc[]? Options { get; set; }
}
