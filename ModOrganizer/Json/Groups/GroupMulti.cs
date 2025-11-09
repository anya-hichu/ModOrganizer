using ModOrganizer.Json.Options;

namespace ModOrganizer.Json.Groups;

// https://github.com/xivdev/Penumbra/blob/318a41fe52ad00ce120d08b2c812e11a6a9b014a/schemas/structs/group_multi.json
public record GroupMulti : Group
{
    public OptionContainer[]? Options { get; set; }
}
