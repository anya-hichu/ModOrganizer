using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Backups;
using ModOrganizer.Json.Penumbra.SortOrders;
using ModOrganizer.Mods;
using ModOrganizer.Tests.Configs;
using ModOrganizer.Tests.Dalamuds.PluginInterfaces;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Json.Readers.Files;
using ModOrganizer.Tests.Mods.ModInterops;
using ModOrganizer.Tests.Systems.DateTimeOffsets;
using ModOrganizer.Tests.TestableClasses;

namespace ModOrganizer.Tests.Backups;

[TestClass]
public class TestBackupManager : ITestableClassTemp
{
    public TestContext TestContext { get; set; }

    [TestMethod]
    public void TestGetFileName()
    {
        var backup = new Backup() { CreatedAt = new(2025, 1, 1, 0, 0, 0, TimeSpan.Zero) };

        var fileName = new BackupManagerBuilder()
            .Build()
            .GetFileName(backup);

        Assert.AreEqual("sort_order.1735689600000.json", fileName);
    }

    [TestMethod]
    public void TestGetFolderPath()
    {
        var tempDirectory = this.CreateResultsTempDirectory();

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
        var tempDirectory = this.CreateResultsTempDirectory();

        var configBackups = new HashSet<Backup>();

        var backupManager = new BackupManagerBuilder()
            .WithConfigBackups(configBackups)
            .WithPluginInterfaceSaveConfigNoop()
            .WithConfigAutoBackupLimit(ushort.MaxValue)
            .WithModInteropSortOrderPath(Path.Combine(tempDirectory, ModInterop.SORT_ORDER_FILE_NAME), exists: true)
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
        var tempDirectory = this.CreateResultsTempDirectory();

        var configBackups = new HashSet<Backup>();

        var backupManager = new BackupManagerBuilder()
            .WithConfigBackups(configBackups)
            .WithPluginInterfaceSaveConfigNoop()
            .WithConfigAutoBackupLimit(ushort.MaxValue)
            .WithModInteropSortOrderPath(Path.Combine(tempDirectory, ModInterop.SORT_ORDER_FILE_NAME), exists: true)
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
        var tempDirectory = this.CreateResultsTempDirectory();

        var builder = new BackupManagerBuilder();
        var configBackups = new HashSet<Backup>();
        var observer = new StubObserver();

        var backupManager = builder
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithConfigBackups(configBackups)
            .WithConfigAutoBackupLimit(ushort.MaxValue)
            .WithModInteropSortOrderPath(Path.Combine(tempDirectory, ModInterop.SORT_ORDER_FILE_NAME))
            .WithPluginInterfaceConfigDirectory(Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer))))
            .Build();

        var message = $"Failed to create {Backup.GetPrettyType(auto)} backup";
        Assert.ThrowsExactly<BackupCreationException>(() => backupManager.Create(auto), message);

        var calls = observer.GetCalls();
        Assert.HasCount(2, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Error), actualMessage => Assert.StartsWith("Caught exception while trying to copy", actualMessage));
        AssertPluginLog.MatchObservedCall(calls[1], nameof(IPluginLog.Error), actualMessage => Assert.AreEqual(message, actualMessage));
    }

    [TestMethod]
    [DoNotParallelize]
    [DataRow(false)]
    [DataRow(true)]
    public void TestTryCreate(bool auto)
    {
        var tempDirectory = this.CreateResultsTempDirectory();
        var configBackups = new HashSet<Backup>();

        var backupManager = new BackupManagerBuilder()
            .WithConfigBackups(configBackups)
            .WithPluginInterfaceSaveConfigNoop()
            .WithConfigAutoBackupLimit(ushort.MaxValue)
            .WithModInteropSortOrderPath(Path.Combine(tempDirectory, ModInterop.SORT_ORDER_FILE_NAME), exists: true)
            .WithPluginInterfaceConfigDirectory(Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer))))
            .Build();

        var nowUtc = DateTimeOffset.UtcNow.AddHours(-1);
        using var _ = new BackupManagerShimsContextBuilder()
            .WithDateTimeOffsetUtcNow(nowUtc)
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
        var tempDirectory = this.CreateResultsTempDirectory();

        var builder = new BackupManagerBuilder();
        var observer = new StubObserver();

        var penumbraDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(Penumbra)));
        var missingSortOrderPath = Path.Combine(penumbraDirectory.FullName, ModInterop.SORT_ORDER_FILE_NAME);

        var backupManager = builder
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithModInteropSortOrderPath(missingSortOrderPath)
            .WithPluginInterfaceConfigDirectory(Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer))))
            .Build();

        var created = backupManager.TryCreate(out var backup, auto);

        Assert.IsFalse(created);
        Assert.IsNull(backup);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Error), value => Assert.StartsWith($"Caught exception while trying to copy [{missingSortOrderPath}]", value));
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void TestTryDeleteWithMissingBackupFile(bool auto)
    {
        var tempDirectory = this.CreateResultsTempDirectory();

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
        var tempDirectory = this.CreateResultsTempDirectory();

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
    [DataRow(false)]
    [DataRow(true)]
    public void TestTryRestore(bool auto)
    {
        var tempDirectory = this.CreateResultsTempDirectory();

        var configBackup = new Backup() { Auto = auto };

        var penumbraDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(Penumbra)));
        var sortOrderPath = Path.Combine(penumbraDirectory.FullName, ModInterop.SORT_ORDER_FILE_NAME);

        var backupManager = new BackupManagerBuilder()
            .WithConfigBackups([configBackup])
            .WithConfigAutoBackupEnabled(false)
            .WithModInteropSortOrderPath(sortOrderPath, exists: true)
            .WithPluginInterfaceSaveConfigNoop()
            .WithPluginInterfaceConfigDirectory(Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer))))
            .Build();

        var backupFileContent = "Old Content";
        File.WriteAllText(backupManager.GetPath(configBackup), backupFileContent);

        var success = backupManager.TryRestore(configBackup);

        Assert.IsTrue(success);
        Assert.AreEqual(backupFileContent, File.ReadAllText(sortOrderPath));
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void TestTryRestoreWithAutoBackup(bool auto)
    {
        var tempDirectory = this.CreateResultsTempDirectory();

        var observer = new StubObserver();

        var configBackup = new Backup() { CreatedAt = DateTimeOffset.UtcNow.AddHours(-1), Auto = auto };

        var penumbraDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(Penumbra)));
        var sortOrderPath = Path.Combine(penumbraDirectory.FullName, ModInterop.SORT_ORDER_FILE_NAME);

        var backupConfigs = new HashSet<Backup>() { configBackup };

        var backupManager = new BackupManagerBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithConfigBackups(backupConfigs)
            .WithConfigAutoBackupEnabled(true)
            .WithConfigAutoBackupLimit(ushort.MaxValue)
            .WithModInteropSortOrderPath(sortOrderPath, exists: true)
            .WithPluginInterfaceSaveConfigNoop()
            .WithPluginInterfaceConfigDirectory(Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer))))
            .Build();

        var backupFileContent = "Old Content";
        File.WriteAllText(backupManager.GetPath(configBackup), backupFileContent);

        var success = backupManager.TryRestore(configBackup);

        Assert.IsTrue(success);
        Assert.AreEqual(backupFileContent, File.ReadAllText(sortOrderPath));

        Assert.HasCount(2, backupConfigs);

        var createdAutoBackup = backupConfigs.ElementAt(1);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Debug), actualMessage => Assert.AreEqual($"Created auto backup [{createdAutoBackup.CreatedAt}] file before restore", actualMessage));
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void TestTryRestoreWithPenumbraReload(bool auto)
    {
        var tempDirectory = this.CreateResultsTempDirectory();

        var observer = new StubObserver();

        var configBackup = new Backup() { Auto = auto };

        var penumbraDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(Penumbra)));
        var sortOrderPath = Path.Combine(penumbraDirectory.FullName, ModInterop.SORT_ORDER_FILE_NAME);

        var backupManager = new BackupManagerBuilder()
            .WithConfigBackups([configBackup])
            .WithConfigAutoBackupEnabled(false)
            .WithModInteropObserver(observer)
            .WithModInteropReloadPenumbra(true)
            .WithModInteropSortOrderPath(sortOrderPath, exists: true)
            .WithPluginInterfaceSaveConfigNoop()
            .WithPluginInterfaceConfigDirectory(Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer))))
            .Build();

        var backupFileContent = "Old Content";
        File.WriteAllText(backupManager.GetPath(configBackup), backupFileContent);

        var success = backupManager.TryRestore(configBackup, reloadPenumbra: true);

        Assert.IsTrue(success);
        Assert.AreEqual(backupFileContent, File.ReadAllText(sortOrderPath));

        var calls = observer.GetCalls();

        Assert.HasCount(2, calls);
        Assert.AreEqual(nameof(IModInterop.GetSortOrderPath), calls[0].StubbedMethod.Name);
        Assert.AreEqual(nameof(IModInterop.ReloadPenumbra), calls[1].StubbedMethod.Name);
    }

    [TestMethod]
    public void TestEnforceLimit()
    {
        var tempDirectory = this.CreateResultsTempDirectory();
        var configBackups = new HashSet<Backup>() { new() };

        var penumbraDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(Penumbra)));
        var sortOrderPath = Path.Combine(penumbraDirectory.FullName, ModInterop.SORT_ORDER_FILE_NAME);

        var backupManager = new BackupManagerBuilder()
            .WithConfigAutoBackupLimit(0)
            .WithConfigBackups(configBackups)
            .WithModInteropSortOrderPath(sortOrderPath, exists: true)
            .WithPluginInterfaceConfigDirectory(Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer))))
            .WithPluginInterfaceSaveConfigNoop()
            .Build();

        var created = backupManager.TryCreate(out var backup, auto: true);

        Assert.IsTrue(created);
        Assert.IsNotNull(backup);
        Assert.HasCount(1, configBackups);
    }

    [TestMethod]
    [DoNotParallelize]
    public void TestTryRead()
    {
        var tempDirectory = this.CreateResultsTempDirectory();

        var backup = new Backup();
        var sortOrder = new SortOrder();

        var backupManager = new BackupManagerBuilder()
            .WithPluginInterfaceConfigDirectory(Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer))))
            .Build();

        using var _ = new BackupManagerShimsContextBuilder()
            .WithFileReaderTryReadFromFile(sortOrder)
            .Build();

        var success = backupManager.TryRead(backup, out var actualSortOrder);

        Assert.IsTrue(success);
        Assert.AreSame(sortOrder, actualSortOrder);
    }
}
