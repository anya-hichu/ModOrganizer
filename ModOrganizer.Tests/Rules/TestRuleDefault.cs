using ModOrganizer.Rules;

namespace ModOrganizer.Tests.Rules;

[TestClass]
public class TestRuleDefault
{
    [TestMethod]
    public void TestGet()
    {
        var ruleDefaults = new RuleDefaults();

        var rules = ruleDefaults.Build();
        Assert.HasCount(16, rules);
    }
}
