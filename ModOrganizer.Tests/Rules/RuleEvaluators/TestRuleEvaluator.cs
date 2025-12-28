using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Mods;
using ModOrganizer.Rules;
using ModOrganizer.Tests.Shared.PluginLogs;

namespace ModOrganizer.Tests.Rules.RuleEvaluators;

[TestClass]
public class TestRuleEvaluator : TestClass
{
    private static readonly string TEST_MOD_DIRECTORY = "Mod Directory";
    private static readonly string TEST_META_NAME = "Mod Name";

    private static readonly ModInfo TEST_MOD_INFO = new()
    {
        Directory = TEST_MOD_DIRECTORY,
        Path = "Mod Path",
        ChangedItems = [],
        Data = new() { FileVersion = 0 },
        Default = new(),
        Meta = new()
        {
            FileVersion = 0,
            Name = TEST_META_NAME
        },
        Groups = []
    };

    [TestMethod]
    public void TestTryEvaluateDisabled()
    {
        var observer = new StubObserver();
        var ruleEvaluator = new RuleEvaluatorBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var rule = new Rule() { Enabled = false };

        var success = ruleEvaluator.TryEvaluate(rule, TEST_MOD_INFO, out var path);

        Assert.IsFalse(success);
        Assert.IsNull(path);

        Assert.IsEmpty(observer.GetCalls());
    }

    [TestMethod]
    public void TestTryEvaluateManyDisabled()
    {
        var observer = new StubObserver();
        var ruleEvaluator = new RuleEvaluatorBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var rule = new Rule() { Enabled = false };

        var success = ruleEvaluator.TryEvaluateMany([rule], TEST_MOD_INFO, out var path);

        Assert.IsFalse(success);
        Assert.IsNull(path);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        var call = calls[0];
        Assert.AreEqual("Debug", call.StubbedMethod.Name);

        var arguments = call.GetArguments();
        Assert.HasCount(2, arguments);
        Assert.AreEqual($"No rule matched mod [{TEST_MOD_DIRECTORY}] with [0] rules enabled out of [1]", arguments[0] as string);
    }

    [TestMethod]
    public void TestTryEvaluateMatching()
    {
        var observer = new StubObserver();
        var ruleEvaluator = new RuleEvaluatorBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var rule = new Rule()
        { 
            Enabled = true,
            MatchExpression = "true",
            PathTemplate = "{{ meta.name }}"
        };

        var success = ruleEvaluator.TryEvaluate(rule, TEST_MOD_INFO, out var path);

        Assert.IsTrue(success);
        Assert.AreEqual(TEST_META_NAME, path);

        Assert.IsEmpty(observer.GetCalls());
    }

    [TestMethod]
    public void TestTryEvaluateManyPriority()
    {
        var observer = new StubObserver();
        var ruleEvaluator = new RuleEvaluatorBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var matchingRulePath = "Rule Path";

        var rules = new List<Rule>()
        { 
            new()
            {
                Priority = 0,
                Enabled = false,
                MatchExpression = "true",
                PathTemplate = "0"
            },
            new()
            {
                Priority = 1,
                Enabled = true,
                MatchExpression = "false",
                PathTemplate = "1"
            },
            new()
            {
                Priority = 2,
                Enabled = true,
                MatchExpression = "true",
                PathTemplate = "2"
            },
            new()
            {
                Priority = 3,
                Enabled = true,
                Path = matchingRulePath,
                MatchExpression = "true",
                PathTemplate = "3"
            },
        }; 
            
        var success = ruleEvaluator.TryEvaluateMany(rules, TEST_MOD_INFO, out var path);

        Assert.IsTrue(success);
        Assert.AreEqual("3", path);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        var call = calls[0];
        Assert.AreEqual("Debug", call.StubbedMethod.Name);

        var arguments = call.GetArguments();
        Assert.HasCount(2, arguments);
        Assert.AreEqual($"Rule [{matchingRulePath}] matched mod [{TEST_MOD_DIRECTORY}] and evaluated to path [{path}]", arguments[0] as string);
    }
}
