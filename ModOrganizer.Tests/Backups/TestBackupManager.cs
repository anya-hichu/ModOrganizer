using ModOrganizer.Backups;
using ModOrganizer.Tests.Shared;

namespace ModOrganizer.Tests.Backups;

[TestClass]
public class TestBackupManager
{
    public TestContext TestContext { get; set; } = null!;

    [TestMethod]
    public void TestTryCreateManualWithMissingSortOrderPath()
    {
        var tempDirectory = TestContext.CreateTestTempDirectory();

        var stubs = new TestBackupManagerStubs(tempDirectory);
        var missingSortOrderPath = Path.Combine(tempDirectory, "sort_order.json");
        stubs.ModInteropStub.GetSortOrderPath = () => missingSortOrderPath;

        var created = stubs.BackupManager.TryCreate(out var backup);

        Assert.IsFalse(created);
        Assert.IsNull(backup);

        var calls = stubs.PluginLogObserver.GetCalls();
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
        var tempDirectory = TestContext.CreateTestTempDirectory();

        var stubs = new TestBackupManagerStubs(tempDirectory);
        var sortOrderPath = Path.Combine(tempDirectory, "sort_order.json");
        File.WriteAllText(sortOrderPath, string.Empty);

        stubs.ModInteropStub.GetSortOrderPath = () => sortOrderPath;
         
        var registeredBackups = new HashSet<Backup>();
        stubs.ConfigStub.BackupsGet = () => registeredBackups;

        var created = stubs.BackupManager.TryCreate(out var backup, manual);

        Assert.IsTrue(created);
        Assert.IsNotNull(backup);
        Assert.AreEqual(backup.Manual, manual);

        Assert.HasCount(1, registeredBackups);
        Assert.AreEqual(backup, registeredBackups.First());
    }
}
