using ModOrganizer.Rules;

namespace ModOrganizer.Configs.Defaults;

public class ConfigDefault(IRuleDefaults ruleDefaults) : IConfigDefault
{
    public IConfig Build() => new Config() { Rules = ruleDefaults.Build() };
}
