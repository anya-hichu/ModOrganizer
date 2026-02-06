using ModOrganizer.Json.Penumbra.Options.Containers;

namespace ModOrganizer.Json.Penumbra.Groups.Multis;

// https://github.com/xivdev/Penumbra/blob/318a41fe52ad00ce120d08b2c812e11a6a9b014a/schemas/structs/group_multi.json
public class GroupMulti : Group
{
    public OptionContainer[]? Options { get; init; }
}
