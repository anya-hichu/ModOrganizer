using ModOrganizer.GameData;
using System.Collections.Generic;

namespace ModOrganizer.Rules;

public static class RuleBuilder
{
    public static readonly int VERSION = 1;

    public static List<Rule> BuildDefaults() => [
        new() {
            Name = "Sets",
            Priority = 2,
            MatchExpression = "changed_items | object.values | array.each @(do; ret $0?.type.value; end) | array.uniq | array.size > 1",
            PathTemplate = "{{ if data.local_tags | array.concat meta.mod_tags | array.contains 'nsfw' }}Nsfw/{{ end }}Sets/{{ meta.name }}"
        },

        Build(FullEquipType.Head),
        Build(FullEquipType.Body),
        Build(FullEquipType.Hands),
        Build(FullEquipType.Legs),
        Build(FullEquipType.Feet),
        Build(FullEquipType.Ears),
        Build(FullEquipType.Neck),
        Build(FullEquipType.Wrists),
        Build(FullEquipType.Finger),

        Build(EmoteCategory.General),
        Build(EmoteCategory.Special),
        Build(EmoteCategory.Expressions)
    ];

    private static Rule Build(FullEquipType type) => new()
    {
        Name = type.ToString(),
        Priority = 1,
        MatchExpression = $"changed_items | object.values | array.each @(do; ret $0?.type?.value; end) | array.uniq == [{(byte)type}]",
        PathTemplate = string.Concat("{{ if data.local_tags | array.concat meta.mod_tags | array.contains 'nsfw' }}Nsfw/{{ end }}", type, "/{{ meta.name }}")
    };

    private static Rule Build(EmoteCategory emoteCategory) => new()
    {
        Name = emoteCategory.ToString(),
        Priority = 0,
        MatchExpression = $"changed_items | object.values | array.each @(do; ret $0?.emote_category?.row_id; end) | array.filter @(do; ret $0 != null; end) | array.uniq == [{(byte)emoteCategory}]",
        PathTemplate = string.Concat("{{ if data.local_tags | array.concat meta.mod_tags | array.contains 'nsfw' }}Nsfw/{{ end }}", emoteCategory, "/{{ meta.name }}")
    };
}
