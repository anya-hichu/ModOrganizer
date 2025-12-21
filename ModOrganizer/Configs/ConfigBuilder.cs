using ModOrganizer.Rules;

namespace ModOrganizer.Configs;

public static class ConfigBuilder
{
    public static Config BuildDefault() => new() { Rules = RuleBuilder.BuildDefaults() };
}
