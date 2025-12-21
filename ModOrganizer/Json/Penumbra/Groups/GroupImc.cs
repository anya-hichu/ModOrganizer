using ModOrganizer.Json.Readers.Penumbra.Manipulations.Metas.Imcs;
using ModOrganizer.Json.Readers.Penumbra.Options.Imcs;

namespace ModOrganizer.Json.Readers.Penumbra.Groups;

// https://github.com/xivdev/Penumbra/blob/318a41fe52ad00ce120d08b2c812e11a6a9b014a/schemas/structs/group_imc.json
public class GroupImc : Group
{
    public bool? AllVariants { get; init; }
    public bool? AllAttributes { get; init; }
    public MetaImcIdentifier? Identifier { get; init; }
    public MetaImcEntry? DefaultEntry { get; init; }
    public OptionImc[]? Options { get; init; }
}
