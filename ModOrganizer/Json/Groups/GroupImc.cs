using ModOrganizer.Json.Manipulations.Metas.Imcs;
using ModOrganizer.Json.Options.Imcs;

namespace ModOrganizer.Json.Groups;

// https://github.com/xivdev/Penumbra/blob/318a41fe52ad00ce120d08b2c812e11a6a9b014a/schemas/structs/group_imc.json
public class GroupImc : Group
{
    public bool? AllVariants { get; init; }
    public bool? AllAttributes { get; init; }
    public MetaImcIdentifier? Identifier { get; init; }
    public MetaImcEntry? DefaultEntry { get; init; }
    public OptionImc[]? Options { get; init; }
}
