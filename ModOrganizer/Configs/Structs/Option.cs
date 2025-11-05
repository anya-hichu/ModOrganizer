namespace ModOrganizer.Configs.Types;

// https://github.com/xivdev/Penumbra/blob/master/schemas/structs/option.json
public class Option
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? Priority { get; set; }
    public string? Image { get; set; }
}
