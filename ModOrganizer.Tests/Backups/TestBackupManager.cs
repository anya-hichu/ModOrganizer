using ModOrganizer.Backups;

namespace ModOrganizer.Tests.Backups;

[TestClass]
public class TestBackupManager : TestClass
{
    [TestMethod]
    public void TestGetFileName()
    {
        var offset = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);
        Assert.AreEqual("sort_order.1735689600000.json", BackupManager.GetFileName(offset));
    }

    [TestMethod]
    public void TestTryCreateWithMissingSortOrderPath()
    {
        var tempDirectory = CreateResultsTempDirectory();

        var fakes = new BackupManagerFakes(tempDirectory);
        var missingSortOrderPath = Path.Combine(tempDirectory, "sort_order.json");
        fakes.ModInterop.GetSortOrderPath = () => missingSortOrderPath;

        var backupManager = new BackupManager(fakes.Clock, fakes.Config, fakes.ModInterop, fakes.PluginInterface, fakes.PluginLog);
        var created = backupManager.TryCreate(out var backup);

        Assert.IsFalse(created);
        Assert.IsNull(backup);

        var calls = fakes.PluginLogObserver.GetCalls();
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
        var tempDirectory = CreateResultsTempDirectory();

        var createdAt = DateTimeOffset.UtcNow.AddHours(-1);

        var fakes = new BackupManagerFakes(tempDirectory);
        var sortOrderPath = Path.Combine(tempDirectory, "sort_order.json");
        File.WriteAllText(sortOrderPath, string.Empty);

        fakes.Clock.GetNowUtc = () => createdAt;
        fakes.ModInterop.GetSortOrderPath = () => sortOrderPath;
         
        var registeredBackups = new HashSet<Backup>();
        fakes.Config.BackupsGet = () => registeredBackups;

        var backupManager = new BackupManager(fakes.Clock, fakes.Config, fakes.ModInterop, fakes.PluginInterface, fakes.PluginLog);
        var created = backupManager.TryCreate(out var backup, manual);

        Assert.IsTrue(created);
        Assert.IsNotNull(backup);
        Assert.AreEqual(createdAt, backup.CreatedAt);
        Assert.AreEqual(backup.Manual, manual);

        Assert.HasCount(1, registeredBackups);
        Assert.AreEqual(backup, registeredBackups.First());

        Assert.IsEmpty(fakes.PluginLogObserver.GetCalls());
    }

    [TestMethod]
    public void TestTryDeleteWithMissingBackupFile()
    {
        var tempDirectory = CreateResultsTempDirectory();

        var fakes = new BackupManagerFakes(tempDirectory);

        var registeredBackup = new Backup() { CreatedAt = DateTimeOffset.UtcNow };
        fakes.Config.BackupsGet = () => [registeredBackup];

        var backupManager = new BackupManager(fakes.Clock, fakes.Config, fakes.ModInterop, fakes.PluginInterface, fakes.PluginLog); 
        var deleted = backupManager.TryDelete(registeredBackup);

        Assert.IsTrue(deleted);

        var calls = fakes.PluginLogObserver.GetCalls();
        Assert.HasCount(1, calls);

        var call = calls[0];
        Assert.AreEqual("Warning", call.StubbedMethod.Name);

        var arguments = call.GetArguments();
        Assert.HasCount(2, arguments);
        Assert.StartsWith($"Failed to delete backup", arguments[0] as string);
    }

    [TestMethod]
    public void TestTryDeleteUnregistered()
    {
        var tempDirectory = CreateResultsTempDirectory();
        var fakes = new BackupManagerFakes(tempDirectory);

        var backupManager = new BackupManager(fakes.Clock, fakes.Config, fakes.ModInterop, fakes.PluginInterface, fakes.PluginLog);

        var registeredBackup = new Backup() { CreatedAt = DateTimeOffset.UtcNow };
        var backupPath = Path.Combine(tempDirectory, backupManager.GetPath(registeredBackup));
        File.WriteAllText(backupPath, string.Empty);
        fakes.Config.BackupsGet = () => [];

        var deleted = backupManager.TryDelete(registeredBackup);

        Assert.IsTrue(deleted);

        var calls = fakes.PluginLogObserver.GetCalls();
        Assert.HasCount(1, calls);

        var call = calls[0];
        Assert.AreEqual("Warning", call.StubbedMethod.Name);

        var arguments = call.GetArguments();
        Assert.HasCount(2, arguments);
        Assert.StartsWith($"Failed to unregister backup from config, ignoring", arguments[0] as string);
    }

    [TestMethod]
    public void TestEnforceLimit()
    {
        var tempDirectory = CreateResultsTempDirectory();
        var fakes = new BackupManagerFakes(tempDirectory);

        var registeredBackups = new HashSet<Backup>();
        fakes.Config.BackupsGet = () => registeredBackups;
        fakes.Config.AutoBackupLimitGet = () => ushort.MinValue;

        var sortOrderPath = Path.Combine(tempDirectory, "sort_order.json");
        File.WriteAllText(sortOrderPath, string.Empty);

        fakes.ModInterop.GetSortOrderPath = () => sortOrderPath;

        var backupManager = new BackupManager(fakes.Clock, fakes.Config, fakes.ModInterop, fakes.PluginInterface, fakes.PluginLog);
        var created = backupManager.TryCreate(out var backup, manual: false);

        Assert.IsTrue(created);
        Assert.IsEmpty(registeredBackups);
    }
}
