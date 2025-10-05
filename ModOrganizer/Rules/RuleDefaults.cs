using ModOrganizer.GameData;
using System.Collections.Generic;
using System.Linq;

namespace ModOrganizer.Rules;

public static class RuleDefaults
{
    public static readonly int VERSION = 1;
    private static readonly List<Rule> RULES = [
        new() {
            Name = "Sets",
            Priority = 5,
            FilterExpression = "changed_items | object.values | array.each @(do; ret $0?.type.value; end) | array.uniq | array.size > 1",
            PathTemplate = "{{ if data.local_tags | array.contains 'nsfw' }}Nsfw/{{ end }}Sets/{{ directory }}"
        },
        Build(FullEquipType.Head),
        Build(FullEquipType.Body),
        Build(FullEquipType.Hands),
        Build(FullEquipType.Legs),
        Build(FullEquipType.Feet),
        Build(FullEquipType.Ears),
        Build(FullEquipType.Neck),
        Build(FullEquipType.Wrists)
    ];

    private static Rule Build(FullEquipType type)
    {
        return new()
        {
            Name = type.ToString(),
            Priority = 4,
            FilterExpression = $"changed_items | object.values | array.each @(do; ret $0?.type?.value; end) | array.uniq == [{(int)type}]",
            PathTemplate = string.Format("{{ if data.local_tags | array.contains 'nsfw' }}Nsfw/{{ end }}{0}/{{ directory }}", type)
        };
    }

    public static List<Rule> Get()
    {
        return [.. RULES.Select(c => c.Clone())];
    }
}
