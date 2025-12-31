using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Mods;
using ModOrganizer.Rules;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Testables;

namespace ModOrganizer.Tests.Rules.RuleEvaluators;

[TestClass]
public class TestRuleEvaluator : ITestableClassTemp
{
    public TestContext TestContext { get; set; }

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
    public void TestTryEvaluateWithDisabled()
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
    public void TestTryEvaluateWithMatchExpressionSyntaxError()
    {
        var observer = new StubObserver();
        var ruleEvaluator = new RuleEvaluatorBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var matchExpression = "Syntax Error";

        var rule = new Rule() 
        {
            Enabled = true, 
            MatchExpression = matchExpression, 
            PathTemplate = "Not Empty" 
        };

        var success = ruleEvaluator.TryEvaluate(rule, TEST_MOD_INFO, out var path);

        Assert.IsFalse(success);
        Assert.IsNull(path);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Error), 
            actualMessage => Assert.AreEqual($"Caught expression while evaluating match expression [{matchExpression}] (<input>(1,1) : error : The function `Syntax` was not found), ignoring", actualMessage));
    }

    [TestMethod]
    public void TestTryEvaluateWithMatchExpressionInvalidType()
    {
        var observer = new StubObserver();
        var ruleEvaluator = new RuleEvaluatorBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var matchExpression = "1";

        var rule = new Rule()
        {
            Enabled = true,
            MatchExpression = matchExpression,
            PathTemplate = "Not Empty"
        };

        var success = ruleEvaluator.TryEvaluate(rule, TEST_MOD_INFO, out var path);

        Assert.IsFalse(success);
        Assert.IsNull(path);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Error), actualMessage => Assert.AreEqual($"Match expression [{matchExpression}] did not evaluate to a boolean, ignoring", actualMessage));
    }

    [TestMethod]
    public void TestTryEvaluateWithPathTemplateSyntaxError()
    {
        var observer = new StubObserver();

        var ruleEvaluator = new RuleEvaluatorBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var pathTemplate = "{{ O }X}";

        var rule = new Rule()
        {
            Enabled = true,
            MatchExpression = "true",
            PathTemplate = pathTemplate
        };

        var success = ruleEvaluator.TryEvaluate(rule, TEST_MOD_INFO, out var path);

        Assert.IsFalse(success);
        Assert.IsNull(path);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Error), 
            actualMessage => Assert.AreEqual($$"""
            Failed to parse path template [{{pathTemplate}}], ignoring: <input>(1,6) : error : Invalid token found `}`. Expecting <EOL>/end of line.
            <input>(1,6) : error : Unexpected } while no matching {
            
            """.ReplaceLineEndings(Environment.NewLine), actualMessage));
    }

    [TestMethod]
    public void TestTryEvaluateWithMatching()
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
    public void TestTryEvaluateManyWithDisabled()
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

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Debug), actualMessage => Assert.AreEqual($"No rule matched mod [{TEST_MOD_DIRECTORY}] with [0] rules enabled out of [1]", actualMessage));
    }

    [TestMethod]
    public void TestTryEvaluateManyWithPriority()
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
            }
        }; 
            
        var success = ruleEvaluator.TryEvaluateMany(rules, TEST_MOD_INFO, out var path);

        Assert.IsTrue(success);
        Assert.AreEqual("3", path);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Debug), 
            actualMessage => Assert.AreEqual($"Rule [{matchingRulePath}] matched mod [{TEST_MOD_DIRECTORY}] and evaluated to path [{path}]", actualMessage));
    }
}
