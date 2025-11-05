using ModOrganizer.Configs.Types;

namespace ModOrganizer.Configs;

// https://github.com/xivdev/Penumbra/blob/master/schemas/group.json
public class Group
{
    public int? Version { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Image { get; set; }

    public int? Page { get; set; }

    public int? Priority { get; set; }

    public string Type { get; set; } = string.Empty;

    public int? DefaultSettings { get; set; }

    public Option[] Options { get; set; } = [];

    public Container[] Containers { get; set; } = [];
}
