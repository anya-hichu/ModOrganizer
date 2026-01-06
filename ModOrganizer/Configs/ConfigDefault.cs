using ModOrganizer.Rules;
using ModOrganizer.Shared;

namespace ModOrganizer.Configs;

public class ConfigDefault(RuleDefaults ruleDefaults) : IBuilder<Config>
{
    public Config Build() => new() { Rules = ruleDefaults.Build() };
}
