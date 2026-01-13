using ModOrganizer.Rules;

namespace ModOrganizer.Tests.Rules;

[TestClass]
public class TestRuleDefault
{
    [TestMethod]
    public void TestBuild()
    {
        var ruleDefaults = new RuleDefaults();

        var rules = ruleDefaults.Build();

        Assert.HasCount(16, rules);
    }
}
