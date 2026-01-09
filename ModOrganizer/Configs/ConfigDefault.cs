using ModOrganizer.Rules;

namespace ModOrganizer.Configs;

public class ConfigDefault(IRuleDefaults ruleDefaults) : IConfigDefault
{
    public IConfig Build() => new Config() { Rules = ruleDefaults.Build() };
}
