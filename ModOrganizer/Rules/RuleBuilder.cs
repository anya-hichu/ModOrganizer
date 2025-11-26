using ModOrganizer.GameData;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace ModOrganizer.Rules;

public static class RuleBuilder
{
    public static readonly int VERSION = 0;

    public static List<Rule> BuildDefaults() => [
        // Equipments
        Build(FullEquipType.Head),
        Build(FullEquipType.Body),
        Build(FullEquipType.Hands),
        Build(FullEquipType.Legs),
        Build(FullEquipType.Feet),
        Build(FullEquipType.Ears),
        Build(FullEquipType.Neck),
        Build(FullEquipType.Wrists),
        Build(FullEquipType.Finger),
        
        Build("Weapons", FullEquipTypeExtensions.WeaponTypes),
        Build("Offhands", FullEquipTypeExtensions.OffhandTypes),
        Build("Tools", FullEquipTypeExtensions.ToolTypes),

        // Emotes
        Build(EmoteCategory.General),
        Build(EmoteCategory.Special),
        Build(EmoteCategory.Expressions),

        // Meh, good enough I guess
        new() {
            Name = "Sets",
            Priority = 0,
            MatchExpression = """
            func categorize(kv)
               parts = []
               if kv.value | object.typeof == "object"
                  if kv.value | object.kind == "ValueTuple<enum, enum, enum, CustomizeValue>"
                     parts = parts | array.add (kv.key | string.split(":") | array.first)

                     # Get customization type out of tuple
                     values = kv.value | object.values
                     parts = parts | array.add values[2]
                  else
                     parts = parts | array.add (kv.value | object.kind)
                     type = kv.value.type
                     if type | object.kind == "enum"
                        parts = parts | array.add (type | object.format null)
                     end
                  end
               else
                  base_key = kv.key | string.split(":") | array.first

                  # Ignore generic customization
                  if base_key != "Customize"
                    parts = parts | array.add base_key
                  end
               end
               ret parts | array.join("#")
            end

            changed_items | array.each @categorize | array.uniq | array.size > 1
            """,
            PathTemplate = "{{ if data.local_tags | array.concat meta.mod_tags | array.contains 'nsfw' }}Nsfw/{{ end }}Sets/{{ meta.name }}"
        },
    ];

    private static Rule Build(FullEquipType type) => new()
    {
        Name = type.ToString(),
        Priority = 3,
        MatchExpression = $"""changed_items | object.values | array.each @(do; ret ($0 | object.kind == "EquipItem") && ($0.type | object.format null == "{type}"); end) | array.uniq == [true]""",
        PathTemplate = string.Concat("{{ if data.local_tags | array.concat meta.mod_tags | array.contains 'nsfw' }}Nsfw/{{ end }}Gears/", type, "/{{ meta.name }}")
    };

    private static Rule Build(string categoryName, IReadOnlyList<FullEquipType> equipTypes) => new()
    {
        Name = categoryName,
        Priority = 2,
        MatchExpression = $"""
        equip_types = {JsonSerializer.Serialize(equipTypes.Select(t => t.ToString()))}
        changed_items | object.values | array.filter @(do; ret $0 | object.typeof == "object"; end) | array.each @(do; ret ($0 | object.kind == "EquipItem") && (equip_types | array.contains ($0.type | object.format null)); end) | array.uniq == [true]
        """,
        PathTemplate = string.Concat("{{ if data.local_tags | array.concat meta.mod_tags | array.contains 'nsfw' }}Nsfw/{{ end }}Gears/", categoryName, "/{{ meta.name }}")
    };

    private static Rule Build(EmoteCategory emoteCategory) => new()
    {
        Name = emoteCategory.ToString(),
        Priority = 1,
        MatchExpression = $"""changed_items | object.values | array.each @(do; ret ($0 | object.kind == "Emote") && ($0.emote_category?.row_id == [{(byte)emoteCategory}]); end) | array.uniq == [true]""",
        PathTemplate = string.Concat("{{ if data.local_tags | array.concat meta.mod_tags | array.contains 'nsfw' }}Nsfw/{{ end }}Emotes/", emoteCategory, "/{{ meta.name }}")
    };
}
