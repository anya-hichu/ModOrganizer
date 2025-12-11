using ModOrganizer.Json.Containers;
using ModOrganizer.Json.Options;

namespace ModOrganizer.Json.Groups;

// https://github.com/xivdev/Penumbra/blob/318a41fe52ad00ce120d08b2c812e11a6a9b014a/schemas/structs/group_combining.json
public class GroupCombining : Group
{
    public Option[]? Options { get; init; }
    public NamedContainer[]? Containers { get; init; }
}
