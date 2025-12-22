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
    [DataRow(false)]
    [DataRow(true)]
    public void TestCreate(bool auto)
    {
        var tempDirectory = CreateResultsTempDirectory();

        var configBackups = new HashSet<Backup>();

        var backupManager = new BackupManagerBuilder()
            .WithConfigBackups(configBackups)
            .WithPluginInterfaceSaveConfigNoop()
            .WithClockNewUtc(DateTimeOffset.UtcNow)
            .WithConfigAutoBackupLimit(ushort.MaxValue)
            .WithModInteropSortOrderPath(Path.Combine(tempDirectory, "sort_order.json"), exists: true)
            .WithPluginInterfaceConfigDirectory(Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer))))
            .Build();

        var backup = backupManager.Create(auto);

        Assert.IsNotNull(backup);
        Assert.AreEqual(auto, backup.Auto);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void TestCreateRecent(bool auto)
    {
        var tempDirectory = CreateResultsTempDirectory();
        
        var configBackups = new HashSet<Backup>();

        var backupManager = new BackupManagerBuilder()
            .WithConfigBackups(configBackups)
            .WithPluginInterfaceSaveConfigNoop()
            .WithClockNewUtc(DateTimeOffset.UtcNow)
            .WithConfigAutoBackupLimit(ushort.MaxValue)
            .WithModInteropSortOrderPath(Path.Combine(tempDirectory, "sort_order.json"), exists: true)
            .WithPluginInterfaceConfigDirectory(Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer))))
            .Build();

        for (var i = 0; i < 10; i++) backupManager.CreateRecent(auto);

        Assert.HasCount(1, configBackups);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void TestCreateWithMissingSortOrder(bool auto)
    {
        var tempDirectory = CreateResultsTempDirectory();

        var builder = new BackupManagerBuilder();
        var configBackups = new HashSet<Backup>();

        var backupManager = builder
            .WithPluginLogDefaults()
            .WithConfigBackups(configBackups)
            .WithClockNewUtc(DateTimeOffset.UtcNow)
            .WithConfigAutoBackupLimit(ushort.MaxValue)
            .WithModInteropSortOrderPath(Path.Combine(tempDirectory, "sort_order.json"))
            .WithPluginInterfaceConfigDirectory(Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer))))
            .Build();

        var message = $"Failed to create {(auto ? "auto" : "manual")} backup";
        Assert.ThrowsExactly<BackupCreationException>(() => backupManager.Create(auto), message);

        var calls = builder.PluginLogObserver.GetCalls();
        Assert.HasCount(2, calls);

        var firstCall = calls[0];
        Assert.AreEqual("Error", firstCall.StubbedMethod.Name);

        var firstArguments = firstCall.GetArguments();
        Assert.HasCount(2, firstArguments);
        Assert.StartsWith("Caught exception while try to copy", firstArguments[0] as string);

        var secondCall = calls[1];
        Assert.AreEqual("Error", secondCall.StubbedMethod.Name);

        var secondArguments = secondCall.GetArguments();
        Assert.HasCount(2, secondArguments);
        Assert.AreEqual($"Failed to create {(auto ? "auto" : "manual")} backup", secondArguments[0] as string);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void TestTryCreate(bool auto)
    {
        var tempDirectory = CreateResultsTempDirectory();

        var builder = new BackupManagerBuilder();
        var nowUtc = DateTimeOffset.UtcNow.AddHours(-1);
        var configBackups = new HashSet<Backup>();

        var backupManager = builder
            .WithClockNewUtc(nowUtc)
            .WithConfigBackups(configBackups)
            .WithPluginInterfaceSaveConfigNoop()
            .WithConfigAutoBackupLimit(ushort.MaxValue)
            .WithModInteropSortOrderPath(Path.Combine(tempDirectory, "sort_order.json"), exists: true)
            .WithPluginInterfaceConfigDirectory(Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer))))
            .Build();

        var created = backupManager.TryCreate(out var backup, auto);

        Assert.IsTrue(created);
        Assert.IsNotNull(backup);
        Assert.AreEqual(nowUtc, backup.CreatedAt);
        Assert.AreEqual(backup.Auto, auto);

        Assert.HasCount(1, configBackups);
        Assert.AreEqual(backup, configBackups.First());
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void TestTryCreateWithMissingSortOrderPath(bool auto)
    {
        var tempDirectory = CreateResultsTempDirectory();
        var builder = new BackupManagerBuilder();

        var missingSortOrderPath = Path.Combine(tempDirectory, "sort_order.json");

        var backupManager = builder
            .WithPluginLogDefaults()
            .WithClockNewUtc(DateTimeOffset.UtcNow)
            .WithModInteropSortOrderPath(missingSortOrderPath)
            .WithPluginInterfaceConfigDirectory(Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer))))
            .Build();

        var created = backupManager.TryCreate(out var backup, auto);

        Assert.IsFalse(created);
        Assert.IsNull(backup);

        var calls = builder.PluginLogObserver.GetCalls();
        Assert.HasCount(1, calls);

        var call = calls[0];
        Assert.AreEqual("Error", call.StubbedMethod.Name);

        var arguments = call.GetArguments();
        Assert.HasCount(2, arguments);
        Assert.StartsWith($"Caught exception while try to copy [{missingSortOrderPath}]", arguments[0] as string);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void TestTryDeleteWithMissingBackupFile(bool auto)
    {
        var tempDirectory = CreateResultsTempDirectory();

        var builder = new BackupManagerBuilder();
        var configBackup = new Backup() { Auto = auto };

        var backupManager = builder
            .WithPluginLogDefaults()
            .WithConfigBackups([configBackup])
            .WithPluginInterfaceSaveConfigNoop()
            .WithPluginInterfaceConfigDirectory(Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer))))
            .Build();

        var deleted = backupManager.TryDelete(configBackup);

        Assert.IsTrue(deleted);

        var calls = builder.PluginLogObserver.GetCalls();
        Assert.HasCount(1, calls);

        var call = calls[0];
        Assert.AreEqual("Warning", call.StubbedMethod.Name);

        var arguments = call.GetArguments();
        Assert.HasCount(2, arguments);
        Assert.StartsWith("Failed to delete backup", arguments[0] as string);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void TestTryDeleteWithMissingBackup(bool auto)
    {
        var tempDirectory = CreateResultsTempDirectory();
        var createdAt = DateTimeOffset.UtcNow;

        var builder = new BackupManagerBuilder();

        var backupManager = builder
            .WithPluginLogDefaults()
            .WithConfigBackups([])
            .WithPluginInterfaceSaveConfigNoop()
            .WithPluginInterfaceConfigDirectory(Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer))))
            .Build();

        var deleted = backupManager.TryDelete(new() { CreatedAt = createdAt, Auto = auto });

        Assert.IsTrue(deleted);

        var calls = builder.PluginLogObserver.GetCalls();
        Assert.HasCount(2, calls);

        var firstCall = calls[0];
        Assert.AreEqual("Warning", firstCall.StubbedMethod.Name);

        var firstArguments = firstCall.GetArguments();
        Assert.HasCount(2, firstArguments);
        Assert.AreEqual($"Failed to delete backup [{createdAt}] file since it does not exists, ignoring", firstArguments[0] as string);

        var secondCall = calls[1];
        Assert.AreEqual("Warning", secondCall.StubbedMethod.Name);

        var secondArguments = secondCall.GetArguments();
        Assert.HasCount(2, secondArguments);
        Assert.AreEqual("Failed to unregister backup from config, ignoring", secondArguments[0] as string);
    }

    [TestMethod]
    public void TestEnforceLimit()
    {
        var tempDirectory = CreateResultsTempDirectory();
        var configBackups = new HashSet<Backup>() { new() };

        var backupManager = new BackupManagerBuilder()
            .WithConfigAutoBackupLimit(0)
            .WithConfigBackups(configBackups)
            .WithClockNewUtc(DateTimeOffset.UtcNow)
            .WithModInteropSortOrderPath(Path.Combine(tempDirectory, "sort_order.json"), exists: true)
            .WithPluginInterfaceConfigDirectory(Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer))))
            .WithPluginInterfaceSaveConfigNoop()
            .Build();

        var created = backupManager.TryCreate(out var backup, auto: true);

        Assert.IsTrue(created);
        Assert.IsNotNull(backup);
        Assert.HasCount(1, configBackups);
    }
}
