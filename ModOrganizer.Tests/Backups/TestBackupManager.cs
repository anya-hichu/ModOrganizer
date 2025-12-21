using ModOrganizer.Backups;
using ModOrganizer.Shared;

namespace ModOrganizer.Tests.Backups;

[TestClass]
public class TestBackupManager
{
    public TestContext TextContext { get; set; } = null!;

    [TestMethod]
    public void TestTryCreateManualWithMissingSortOrderPath()
    {
        using var state = new TestBackupManagerState();

        var missingSortOrderPath = "sort_order.json";
        state.ModInteropStub.GetSortOrderPath = () => missingSortOrderPath;

        var created = state.BackupManager.TryCreate(out var backup);

        Assert.IsFalse(created);
        Assert.IsNull(backup);

        var calls = state.PluginLogObserver.GetCalls();
        Assert.HasCount(1, calls);

        var call = calls[0];
        Assert.AreEqual("Error", call.StubbedMethod.Name);

        var arguments = call.GetArguments();
        Assert.HasCount(2, arguments);
        Assert.StartsWith($"Caught exception while try to copy [{missingSortOrderPath}]", arguments[0] as string);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void TestTryCreate(bool manual)
    {
        using var state = new TestBackupManagerState();

        var sortOrderPath = $"{testName}_sort_order_{manual}.json";
        using var _ = new RaiiGuard(() => File.WriteAllText(sortOrderPath, string.Empty), () => File.Delete(sortOrderPath));

        state.ModInteropStub.GetSortOrderPath = () => sortOrderPath;
         
        var registeredBackups = new HashSet<Backup>();
        state.ConfigStub.BackupsGet = () => registeredBackups;
        state.ConfigStub.AutoBackupLimitGet = () => 1;

        var created = state.BackupManager.TryCreate(out var backup, manual);

        Assert.IsTrue(created);
        Assert.IsNotNull(backup);
        Assert.AreEqual(backup.Manual, manual);

        Assert.HasCount(1, registeredBackups);
        Assert.AreEqual(backup, registeredBackups.First());
    }
}
