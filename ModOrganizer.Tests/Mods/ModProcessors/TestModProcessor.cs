using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Mods;
using ModOrganizer.Tests.Configs;
using ModOrganizer.Tests.Mods.ModInterops;
using ModOrganizer.Tests.Rules.RuleEvaluators;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Testables;
using Penumbra.Api.Enums;
using ModOrganizer.Backups;
using ModOrganizer.Tests.Backups;

namespace ModOrganizer.Tests.Mods.ModProcessors;

[TestClass]
public class TestModProcessor : ITestableClassTemp
{
    public TestContext TestContext { get; set; }

    private static readonly string TEST_MOD_DIRECTORY = "Mod Directory";

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
            Name = "Mod Name"
        },
        Groups = []
    };

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void TestTryProcessWithoutModInfo(bool dryRun)
    {
        var modProcessor = new ModProcessorBuilder()
            .WithModInteropTryGetModInfo(null)
            .Build();

        var success = modProcessor.TryProcess(string.Empty, out var newModPath, dryRun);

        Assert.IsFalse(success);
        Assert.IsNull(newModPath);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void TestTryProcessWithoutMatchingRules(bool dryRun)
    {
        var observer = new StubObserver();

        var modProcessor = new ModProcessorBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithModInteropTryGetModInfo(TEST_MOD_INFO)
            .WithConfigRules([])
            .WithRuleEvaluatorTryEvaluateMany(null)
            .Build();

        var success = modProcessor.TryProcess(TEST_MOD_DIRECTORY, out var newModPath, dryRun);

        Assert.IsFalse(success);
        Assert.IsNull(newModPath);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), actualMessage => Assert.AreEqual($"No rule matched mod [{TEST_MOD_DIRECTORY}]", actualMessage));
    }

    [TestMethod]
    public void TestTryProcessWithAutoBackup()
    {
        var observer = new StubObserver();

        var newModPath = "New Mod Path";

        var modProcessor = new ModProcessorBuilder()
            .WithConfigRules([])
            .WithConfigAutoBackupEnabled(true)
            .WithBackupManagerDefaults()
            .WithBackupManagerObserver(observer)
            .WithRuleEvaluatorTryEvaluateMany(newModPath)
            .WithModInteropTryGetModInfo(TEST_MOD_INFO)
            .WithModInteropSetModPath(PenumbraApiEc.Success)
            .Build();

        var success = modProcessor.TryProcess(TEST_MOD_DIRECTORY, out var path);

        Assert.IsTrue(success);
        Assert.AreEqual(newModPath, path);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        var call = calls[0];
        Assert.AreEqual(nameof(IBackupManager.CreateRecent), call.StubbedMethod.Name);

        var arguments = call.GetArguments();
        Assert.HasCount(1, arguments);
        Assert.IsTrue(arguments[0] is bool auto && auto);
    }

    [TestMethod]
    public void TestTryProcessWithRenameError()
    {
        var observer = new StubObserver();

        var newModPath = "New Mod Path";

        var modProcessor = new ModProcessorBuilder()
            .WithConfigRules([])
            .WithConfigAutoBackupEnabled(false)
            .WithRuleEvaluatorTryEvaluateMany(newModPath)
            .WithModInteropTryGetModInfo(TEST_MOD_INFO)
            .WithModInteropSetModPath(PenumbraApiEc.PathRenameFailed)
            .Build();

        var success = modProcessor.TryProcess(TEST_MOD_DIRECTORY, out var path);

        Assert.IsFalse(success);
        Assert.AreEqual(newModPath, path);
    }

    [TestMethod]
    public void TestTryProcessWithDryRun()
    {
        var newModPath = "New Mod Path";

        var modProcessor = new ModProcessorBuilder()
            .WithModInteropTryGetModInfo(TEST_MOD_INFO)
            .WithConfigRules([])
            .WithRuleEvaluatorTryEvaluateMany(newModPath)
            .Build();

        var success = modProcessor.TryProcess(TEST_MOD_DIRECTORY, out var path, dryRun: true);
        
        Assert.IsTrue(success);
        Assert.AreEqual(newModPath, path);
    }

    [TestMethod]
    public void TestTryProcess()
    {
        var newModPath = "New Mod Path";

        var modProcessor = new ModProcessorBuilder()
            .WithConfigRules([])
            .WithConfigAutoBackupEnabled(false)
            .WithRuleEvaluatorTryEvaluateMany(newModPath)
            .WithModInteropTryGetModInfo(TEST_MOD_INFO)
            .WithModInteropSetModPath(PenumbraApiEc.Success)
            .Build();

        var success = modProcessor.TryProcess(TEST_MOD_DIRECTORY, out var path);

        Assert.IsTrue(success);
        Assert.AreEqual(newModPath, path);
    }
}
