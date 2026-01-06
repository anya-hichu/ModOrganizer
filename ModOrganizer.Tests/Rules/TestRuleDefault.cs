using ModOrganizer.Rules;
using ModOrganizer.Tests.Testables;

namespace ModOrganizer.Tests.Rules;

[TestClass]
public class TestRuleDefault : ITestableClassTemp
{
    public TestContext TestContext { get; set; }

    [TestMethod]
    public void TestGet()
    {
        var ruleDefaults = new RuleDefaults();

        var rules = ruleDefaults.Build();
        Assert.HasCount(16, rules);
    }
}
