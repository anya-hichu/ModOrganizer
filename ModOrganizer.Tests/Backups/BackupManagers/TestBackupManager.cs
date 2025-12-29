using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Backups;
using ModOrganizer.Tests.Configs;
using ModOrganizer.Tests.Mods.ModInterops;
using ModOrganizer.Tests.Shared.Clock;
using ModOrganizer.Tests.Shared.PluginInterfaces;
using ModOrganizer.Tests.Shared.PluginLogs;

namespace ModOrganizer.Tests.Backups.BackupManagers;

[TestClass]
public class TestBackupManager : TestClass
{
    [TestMethod]
    public void TestGetFileName()
    {
        var backupManager = new BackupManagerBuilder().Build();

        var backup = new Backup() { CreatedAt = new(2025, 1, 1, 0, 0, 0, TimeSpan.Zero) };

        Assert.AreEqual("sort_order.1735689600000.json", backupManager.GetFileName(backup));
    }

    [TestMethod]
    public void TestGetFolderPath()
    {
        var tempDirectory = CreateResultsTempDirectory();

        var configDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer)));

        var backupManager = new BackupManagerBuilder()
            .WithPluginInterfaceConfigDirectory(configDirectory)
            .Build();

        Assert.AreEqual(configDirectory.FullName, backupManager.GetFolderPath());
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

        var configBackup = configBackups.ElementAt(0);
        Assert.AreEqual(auto, configBackup.Auto);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void TestCreateWithMissingSortOrder(bool auto)
    {
        var tempDirectory = CreateResultsTempDirectory();

        var builder = new BackupManagerBuilder();
        var configBackups = new HashSet<Backup>();
        var observer = new StubObserver();

        var backupManager = builder
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithConfigBackups(configBackups)
            .WithClockNewUtc(DateTimeOffset.UtcNow)
            .WithConfigAutoBackupLimit(ushort.MaxValue)
            .WithModInteropSortOrderPath(Path.Combine(tempDirectory, "sort_order.json"))
            .WithPluginInterfaceConfigDirectory(Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer))))
            .Build();

        var message = $"Failed to create {Backup.GetPrettyType(auto)} backup";
        Assert.ThrowsExactly<BackupCreationException>(() => backupManager.Create(auto), message);

        var calls = observer.GetCalls();
        Assert.HasCount(2, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Error), actualMessage => Assert.StartsWith("Caught exception while try to copy", actualMessage));
        AssertPluginLog.MatchObservedCall(calls[1], nameof(IPluginLog.Error), actualMessage => Assert.AreEqual(message, actualMessage));
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
        var observer = new StubObserver();
        var missingSortOrderPath = Path.Combine(tempDirectory, "sort_order.json");

        var backupManager = builder
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithClockNewUtc(DateTimeOffset.UtcNow)
            .WithModInteropSortOrderPath(missingSortOrderPath)
            .WithPluginInterfaceConfigDirectory(Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer))))
            .Build();

        var created = backupManager.TryCreate(out var backup, auto);

        Assert.IsFalse(created);
        Assert.IsNull(backup);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Error), value => Assert.StartsWith($"Caught exception while try to copy [{missingSortOrderPath}]", value));
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void TestTryDeleteWithMissingBackupFile(bool auto)
    {
        var tempDirectory = CreateResultsTempDirectory();

        var builder = new BackupManagerBuilder();
        var configBackup = new Backup() { Auto = auto };
        var observer = new StubObserver();

        var backupManager = builder
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithConfigBackups([configBackup])
            .WithPluginInterfaceSaveConfigNoop()
            .WithPluginInterfaceConfigDirectory(Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer))))
            .Build();

        var deleted = backupManager.TryDelete(configBackup);

        Assert.IsTrue(deleted);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), value => Assert.StartsWith($"Failed to delete {Backup.GetPrettyType(auto)} backup", value));
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void TestTryDeleteWithMissingBackup(bool auto)
    {
        var tempDirectory = CreateResultsTempDirectory();

        var builder = new BackupManagerBuilder();
        var observer = new StubObserver();

        var backupManager = builder
            .WithConfigBackups([])
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithPluginInterfaceSaveConfigNoop()
            .WithPluginInterfaceConfigDirectory(Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer))))
            .Build();

        var createdAt = DateTimeOffset.UtcNow;
        var deleted = backupManager.TryDelete(new() { CreatedAt = createdAt, Auto = auto });

        Assert.IsTrue(deleted);

        var calls = observer.GetCalls();
        Assert.HasCount(2, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), 
            actualMessage => Assert.AreEqual($"Failed to delete {Backup.GetPrettyType(auto)} backup [{createdAt}] file since it does not exists, ignoring", actualMessage));

        AssertPluginLog.MatchObservedCall(calls[1], nameof(IPluginLog.Warning), 
            actualMessage => Assert.AreEqual($"Failed to unregister {Backup.GetPrettyType(auto)} backup from config, ignoring", actualMessage));
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
