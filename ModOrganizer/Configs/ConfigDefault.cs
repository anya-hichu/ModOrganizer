using ModOrganizer.Rules;

namespace ModOrganizer.Configs;

public class ConfigDefault(IRuleDefaults ruleDefaults) : IConfigDefault
{
    public Config Build() => new() { Rules = ruleDefaults.Build() };
}
