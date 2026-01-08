using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.DefaultMods;
using ModOrganizer.Json.Penumbra.LocalModDatas;
using ModOrganizer.Json.Penumbra.ModMetas;
using ModOrganizer.Json.Penumbra.SortOrders;
using ModOrganizer.Mods;
using ModOrganizer.Tests.Dalamuds.CommandManagers;
using ModOrganizer.Tests.Dalamuds.PenumbraApis;
using ModOrganizer.Tests.Dalamuds.PluginInterfaces;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Json.Readers.Files;
using ModOrganizer.Tests.Systems;
using ModOrganizer.Tests.Testables;
using Penumbra.Api.Enums;

namespace ModOrganizer.Tests.Mods.ModInterops;

[TestClass]
public class TestModInterop : ITestableClassTemp
{
    public TestContext TestContext { get; set; }

    [TestMethod]
    public void TestOnModAdded()
    {
        var observer = new StubObserver();

        var tempDirectory = this.CreateResultsTempDirectory();

        var configDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer)));
        var penumbraConfigDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(Penumbra)));

        Directory.CreateDirectory(Path.Combine(penumbraConfigDirectory.FullName, "mod_data"));

        var modsDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, "ModsDirectory"));

        var registeredActions = new HashSet<Action<string>>();

        var modInterop = new ModInteropBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithPenumbraApiModAdded(registeredActions)
            .WithPenumbraApiModMovedNoop()
            .WithPenumbraApiModDirectoryChangedNoop()
            .WithPenumbraApiGetModList([])
            .WithPenumbraApiGetChangedItems([])
            .WithPenumbraApiGetModDirectory(modsDirectory)
            .WithPluginInterfaceInjectObject(false)
            .WithPenumbraApiSetModPath(PenumbraApiEc.Success)
            .WithPluginInterfaceConfigDirectory(configDirectory)
            .Build();

        Assert.HasCount(1, registeredActions);
        var registeredAction = registeredActions.ElementAt(0);

        var modDirectory = "Mod Directory";

        var onModAddedObserver = new StubObserver();
        modInterop.OnModAdded += StubAction.WithObserver<string>(onModAddedObserver);

        var beforeCalls = observer.GetCalls();
        Assert.HasCount(1, beforeCalls);
        AssertPluginLog.MatchObservedCall(beforeCalls[0], nameof(IPluginLog.Debug), 
            actualMessage => Assert.AreEqual("Created mod file system watchers", actualMessage));

        registeredAction.Invoke(modDirectory);

        var onModAddedCalls = onModAddedObserver.GetCalls();
        Assert.HasCount(1, onModAddedCalls);
        Assert.AreEqual(modDirectory, onModAddedCalls[0].GetArguments()[0] as string);

        var afterCalls = observer.GetCalls();
        Assert.HasCount(5, afterCalls);
        AssertPluginLog.MatchObservedCall(afterCalls[1], nameof(IPluginLog.Debug), 
            actualMessage => Assert.AreEqual($"Received mod added event [{modDirectory}]", actualMessage));
        AssertPluginLog.MatchObservedCall(afterCalls[2], nameof(IPluginLog.Debug), 
            actualMessage => Assert.AreEqual($"Invalidating caches for mod [{modDirectory}]", actualMessage));
        AssertPluginLog.MatchObservedCall(afterCalls[3], nameof(IPluginLog.Debug), 
            actualMessage => Assert.AreEqual($"Invalidating sort order data cache", actualMessage));
        AssertPluginLog.MatchObservedCall(afterCalls[4], nameof(IPluginLog.Debug), 
            actualMessage => Assert.AreEqual($"Invalidating mod info cache [{modDirectory}]", actualMessage));
    }

    [TestMethod]
    public void TestOnModDeleted()
    {
        var observer = new StubObserver();

        var tempDirectory = this.CreateResultsTempDirectory();

        var configDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer)));
        var penumbraConfigDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(Penumbra)));

        Directory.CreateDirectory(Path.Combine(penumbraConfigDirectory.FullName, "mod_data"));

        var modsDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, "ModsDirectory"));

        var registeredActions = new HashSet<Action<string>>();

        var modInterop = new ModInteropBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithPenumbraApiModDeleted(registeredActions)
            .WithPenumbraApiModMovedNoop()
            .WithPenumbraApiModDirectoryChangedNoop()
            .WithPenumbraApiGetModList([])
            .WithPenumbraApiGetChangedItems([])
            .WithPenumbraApiGetModDirectory(modsDirectory)
            .WithPenumbraApiSetModPath(PenumbraApiEc.Success)
            .WithPluginInterfaceInjectObject(false)
            .WithPluginInterfaceConfigDirectory(configDirectory)
            .Build();

        Assert.HasCount(1, registeredActions);
        var registeredAction = registeredActions.ElementAt(0);

        var modDirectory = "Mod Directory";

        var onModDeletedObserver = new StubObserver();
        modInterop.OnModDeleted += StubAction.WithObserver<string>(onModDeletedObserver);

        var beforeCalls = observer.GetCalls();
        Assert.HasCount(1, beforeCalls);
        AssertPluginLog.MatchObservedCall(beforeCalls[0], nameof(IPluginLog.Debug), 
            actualMessage => Assert.AreEqual("Created mod file system watchers", actualMessage));

        registeredAction.Invoke(modDirectory);

        var onModDeletedCalls = onModDeletedObserver.GetCalls();
        Assert.HasCount(1, onModDeletedCalls);
        Assert.AreEqual(modDirectory, onModDeletedCalls[0].GetArguments()[0] as string);

        var afterCalls = observer.GetCalls();
        Assert.HasCount(5, afterCalls);
        AssertPluginLog.MatchObservedCall(afterCalls[1], nameof(IPluginLog.Debug), 
            actualMessage => Assert.AreEqual($"Received mod deleted event [{modDirectory}]", actualMessage));
        AssertPluginLog.MatchObservedCall(afterCalls[2], nameof(IPluginLog.Debug), 
            actualMessage => Assert.AreEqual($"Invalidating caches for mod [{modDirectory}]", actualMessage));
        AssertPluginLog.MatchObservedCall(afterCalls[3], nameof(IPluginLog.Debug), 
            actualMessage => Assert.AreEqual($"Invalidating sort order data cache", actualMessage));
        AssertPluginLog.MatchObservedCall(afterCalls[4], nameof(IPluginLog.Debug), 
            actualMessage => Assert.AreEqual($"Invalidating mod info cache [{modDirectory}]", actualMessage));
    }

    [TestMethod]
    public void TestOnModMoved()
    {
        var observer = new StubObserver();

        var tempDirectory = this.CreateResultsTempDirectory();

        var configDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer)));
        var penumbraConfigDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(Penumbra)));

        Directory.CreateDirectory(Path.Combine(penumbraConfigDirectory.FullName, "mod_data"));

        var modsDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, "ModsDirectory"));

        var registeredActions = new HashSet<Action<string, string>>();

        var modInterop = new ModInteropBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithPenumbraApiModMoved(registeredActions)
            .WithPenumbraApiModAddedOrDeletedNoop()
            .WithPenumbraApiModDirectoryChangedNoop()
            .WithPenumbraApiGetModList([])
            .WithPenumbraApiGetChangedItems([])
            .WithPenumbraApiGetModDirectory(modsDirectory)
            .WithPluginInterfaceInjectObject(false)
            .WithPenumbraApiSetModPath(PenumbraApiEc.Success)
            .WithPluginInterfaceConfigDirectory(configDirectory)
            .Build();

        Assert.HasCount(1, registeredActions);
        var registeredAction = registeredActions.ElementAt(0);

        var modDirectory = "Mod Directory";
        var newModDirectory = "New Mod Directory";

        var onModMovedObserver = new StubObserver();
        modInterop.OnModMoved += StubAction.WithObserver<string, string>(onModMovedObserver);

        var beforeCalls = observer.GetCalls();
        Assert.HasCount(1, beforeCalls);
        AssertPluginLog.MatchObservedCall(beforeCalls[0], nameof(IPluginLog.Debug), 
            actualMessage => Assert.AreEqual("Created mod file system watchers", actualMessage));

        registeredAction.Invoke(modDirectory, newModDirectory);

        var onModMovedCalls = onModMovedObserver.GetCalls();
        Assert.HasCount(1, onModMovedCalls);

        var onModMovedArguments = onModMovedCalls[0].GetArguments();
        Assert.AreEqual(modDirectory, onModMovedArguments[0] as string);
        Assert.AreEqual(newModDirectory, onModMovedArguments[1] as string);

        var afterCalls = observer.GetCalls();
        Assert.HasCount(5, afterCalls);
        AssertPluginLog.MatchObservedCall(afterCalls[1], nameof(IPluginLog.Debug), 
            actualMessage => Assert.AreEqual($"Received mod moved event [{modDirectory}] to [{newModDirectory}]", actualMessage));
        AssertPluginLog.MatchObservedCall(afterCalls[2], nameof(IPluginLog.Debug), 
            actualMessage => Assert.AreEqual($"Invalidating caches for mod [{modDirectory}]", actualMessage));
        AssertPluginLog.MatchObservedCall(afterCalls[3], nameof(IPluginLog.Debug), 
            actualMessage => Assert.AreEqual($"Invalidating sort order data cache", actualMessage));
        AssertPluginLog.MatchObservedCall(afterCalls[4], nameof(IPluginLog.Debug), 
            actualMessage => Assert.AreEqual($"Invalidating mod info cache [{modDirectory}]", actualMessage));
    }

    [TestMethod]
    public void TestOnModDirectoryChanged()
    {
        var observer = new StubObserver();

        var tempDirectory = this.CreateResultsTempDirectory();

        var configDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer)));
        var penumbraConfigDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(Penumbra)));

        Directory.CreateDirectory(Path.Combine(penumbraConfigDirectory.FullName, "mod_data"));

        var modsDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, "ModsDirectory"));

        var registeredActions = new HashSet<Action<string, bool>>();

        var modInterop = new ModInteropBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithPenumbraApiModMovedNoop()
            .WithPenumbraApiModAddedOrDeletedNoop()
            .WithPenumbraApiModDirectoryChanged(registeredActions)
            .WithPenumbraApiGetModList([])
            .WithPenumbraApiGetChangedItems([])
            .WithPenumbraApiGetModDirectory(modsDirectory)
            .WithPluginInterfaceInjectObject(false)
            .WithPenumbraApiSetModPath(PenumbraApiEc.Success)
            .WithPluginInterfaceConfigDirectory(configDirectory)
            .Build();

        Assert.HasCount(1, registeredActions);
        var registeredAction = registeredActions.ElementAt(0);

        var beforeCalls = observer.GetCalls();
        Assert.HasCount(1, beforeCalls);
        AssertPluginLog.MatchObservedCall(beforeCalls[0], nameof(IPluginLog.Debug),
            actualMessage => Assert.AreEqual("Created mod file system watchers", actualMessage));

        var newModsDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, "NewModsDirectory"));
        registeredAction.Invoke(newModsDirectory.FullName, true);

        var afterCalls = observer.GetCalls();
        Assert.HasCount(7, afterCalls);
        AssertPluginLog.MatchObservedCall(afterCalls[1], nameof(IPluginLog.Debug),
            actualMessage => Assert.AreEqual($"Received mod directory changed [{newModsDirectory}]", actualMessage));
        AssertPluginLog.MatchObservedCall(afterCalls[2], nameof(IPluginLog.Debug),
            actualMessage => Assert.AreEqual("Disposed mod file system watchers", actualMessage));
        AssertPluginLog.MatchObservedCall(afterCalls[3], nameof(IPluginLog.Debug),
            actualMessage => Assert.AreEqual("Created mod file system watchers", actualMessage));
        AssertPluginLog.MatchObservedCall(afterCalls[4], nameof(IPluginLog.Debug),
            actualMessage => Assert.AreEqual("Invalidating all caches", actualMessage));
        AssertPluginLog.MatchObservedCall(afterCalls[5], nameof(IPluginLog.Debug),
            actualMessage => Assert.AreEqual("Invalidating sort order data cache", actualMessage));
        AssertPluginLog.MatchObservedCall(afterCalls[6], nameof(IPluginLog.Debug),
            actualMessage => Assert.AreEqual("Invalidating mod info caches (count: 0)", actualMessage));
    }

    [TestMethod]
    [DoNotParallelize]
    public void TestOnSortOrderChanged()
    {
        var observer = new StubObserver();

        var tempDirectory = this.CreateResultsTempDirectory();

        var configDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer)));
        var penumbraConfigDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(Penumbra)));

        Directory.CreateDirectory(Path.Combine(penumbraConfigDirectory.FullName, "mod_data"));

        var modsDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, "ModsDirectory"));

        var modInterop = new ModInteropBuilder()
            .WithPluginLogDefaults()
            .WithPenumbraApiModMovedNoop()
            .WithPenumbraApiModAddedOrDeletedNoop()
            .WithPenumbraApiModDirectoryChangedNoop()
            .WithPenumbraApiGetModList([])
            .WithPenumbraApiGetChangedItems([])
            .WithPenumbraApiGetModDirectory(modsDirectory)
            .WithPluginInterfaceInjectObject(false)
            .WithPenumbraApiSetModPath(PenumbraApiEc.Success)
            .WithPluginInterfaceConfigDirectory(configDirectory)
            .Build();

        using var _ = new ModInteropShimsContextBuilder()
            .WithFileReaderTryReadFromFile(null as SortOrder)
            .Build();

        modInterop.OnSortOrderChanged += StubAction.WithObserver(observer);

        var sortOrder = modInterop.GetSortOrder();

        Assert.IsEmpty(sortOrder.Data);
        Assert.HasCount(1, observer.GetCalls());
    }

    [TestMethod]
    [DoNotParallelize]
    public async Task TestToggleFsWatchers()
    {
        // Idea: Maybe shim fs watchers to be able to properly "wait" on them

        var observer = new StubObserver();

        var tempDirectory = this.CreateResultsTempDirectory();

        var configDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer)));
        var penumbraConfigDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(Penumbra)));

        Directory.CreateDirectory(Path.Combine(penumbraConfigDirectory.FullName, "mod_data"));

        var modsDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, "ModsDirectory"));

        var modInterop = new ModInteropBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithPenumbraApiModMovedNoop()
            .WithPenumbraApiModAddedOrDeletedNoop()
            .WithPenumbraApiModDirectoryChangedNoop()
            .WithPenumbraApiGetModList([])
            .WithPenumbraApiGetChangedItems([])
            .WithPenumbraApiGetModDirectory(modsDirectory)
            .WithPluginInterfaceInjectObject(false)
            .WithPenumbraApiSetModPath(PenumbraApiEc.Success)
            .WithPluginInterfaceConfigDirectory(configDirectory)
            .Build();

        modInterop.ToggleFsWatchers(false);

        var beforeToggleCalls = observer.GetCalls();
        Assert.HasCount(2, beforeToggleCalls);
        AssertPluginLog.MatchObservedCall(beforeToggleCalls[0], nameof(IPluginLog.Debug),
            actualMessage => Assert.AreEqual("Created mod file system watchers", actualMessage));
        AssertPluginLog.MatchObservedCall(beforeToggleCalls[1], nameof(IPluginLog.Debug),
            actualMessage => Assert.AreEqual("Disabled file system watchers", actualMessage));

        File.WriteAllText(modInterop.GetSortOrderPath(), "{}");

        Assert.HasCount(2, observer.GetCalls());

        modInterop.ToggleFsWatchers(true);

        var afterToggleCalls = observer.GetCalls();
        Assert.HasCount(3, observer.GetCalls());

        AssertPluginLog.MatchObservedCall(afterToggleCalls[2], nameof(IPluginLog.Debug),
            actualMessage => Assert.AreEqual("Enabled file system watchers", actualMessage));

        File.WriteAllText(modInterop.GetSortOrderPath(), "{ }");

        try
        {
            var expectedCount = 11;

            var task = Task.Run(() => 
            {
                while (observer.GetCalls().Length < expectedCount) Thread.Sleep(1);
            }, 
            TestContext.CancellationToken);
            
            await task.WaitAsync(TimeSpan.FromMilliseconds(50), TestContext.CancellationToken);

            Assert.HasCount(expectedCount, observer.GetCalls());
        }
        catch (TimeoutException)
        {
            Assert.Fail("Timed out while waiting for file system watchers");
        }
    }

    [TestMethod]
    [DoNotParallelize]
    public void TestTryGetModInfo()
    {
        var observer = new StubObserver();

        var tempDirectory = this.CreateResultsTempDirectory();

        var configDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer)));
        var penumbraConfigDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(Penumbra)));

        Directory.CreateDirectory(Path.Combine(penumbraConfigDirectory.FullName, "mod_data"));

        var modsDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, "ModsDirectory"));

        var modDirectory = "Mod Directory";
        var modPath = "Mod Path";


        var modInterop = new ModInteropBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithPenumbraApiModMovedNoop()
            .WithPenumbraApiModAddedOrDeletedNoop()
            .WithPenumbraApiModDirectoryChangedNoop()
            .WithPenumbraApiGetModList([])
            .WithPenumbraApiGetChangedItems([])
            .WithPenumbraApiGetModDirectory(modsDirectory)
            .WithPenumbraApiSetModPath(PenumbraApiEc.Success)
            .WithPluginInterfaceInjectObject(false)
            .WithPluginInterfaceConfigDirectory(configDirectory)
            .Build();

        var localModData = new LocalModData() { FileVersion = 0 };
        var defaultMod = new DefaultMod();
        var modMeta = new ModMeta() { FileVersion = 0, Name = "Mod Name" };
        var sortOrder = new SortOrder() { Data = { { modDirectory, modPath } } };

        using var _ = new ModInteropShimsContextBuilder()
            .WithFileReaderTryReadFromFile(sortOrder)
            .WithFileReaderTryReadFromFile(localModData)
            .WithFileReaderTryReadFromFile(defaultMod)
            .WithFileReaderTryReadFromFile(modMeta)
            .Build();

        var beforeCalls = observer.GetCalls();
        Assert.HasCount(1, beforeCalls);
        AssertPluginLog.MatchObservedCall(beforeCalls[0], nameof(IPluginLog.Debug),
            actualMessage => Assert.AreEqual("Created mod file system watchers", actualMessage));

        var success = modInterop.TryGetModInfo(modDirectory, out var modInfo);

        var afterCalls = observer.GetCalls();
        Assert.HasCount(2, afterCalls);
        AssertPluginLog.MatchObservedCall(afterCalls[1], nameof(IPluginLog.Debug),
            actualMessage => Assert.AreEqual("Loaded [SortOrder] cache (count: 1)", actualMessage));

        Assert.IsTrue(success);
        Assert.IsNotNull(modInfo);

        Assert.AreEqual(modInfo.Directory, modDirectory);
        Assert.AreEqual(modInfo.Path, modPath);

        Assert.AreSame(modInfo.Data, localModData);
        Assert.AreSame(modInfo.Default, defaultMod);
        Assert.AreSame(modInfo.Meta, modMeta);
    }

    [TestMethod]
    public void TestGetModList()
    {
        var tempDirectory = this.CreateResultsTempDirectory();

        var configDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer)));
        var penumbraConfigDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(Penumbra)));

        Directory.CreateDirectory(Path.Combine(penumbraConfigDirectory.FullName, "mod_data"));

        var modsDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, "ModsDirectory"));

        var modDirectory = "Mod Directory";
        var modName = "Mod Name";

        var modList = new Dictionary<string, string>() { { modDirectory, modName } };

        var modInterop = new ModInteropBuilder()
            .WithPluginLogDefaults()
            .WithPenumbraApiModMovedNoop()
            .WithPenumbraApiGetModList(modList)
            .WithPenumbraApiGetChangedItems([])
            .WithPenumbraApiModAddedOrDeletedNoop()
            .WithPenumbraApiModDirectoryChangedNoop()
            .WithPenumbraApiGetModDirectory(modsDirectory)
            .WithPenumbraApiSetModPath(PenumbraApiEc.Success)
            .WithPluginInterfaceInjectObject(false)
            .WithPluginInterfaceConfigDirectory(configDirectory)
            .Build();

        var actualModList = modInterop.GetModList();

        Assert.HasCount(1, actualModList);
        Assert.AreEqual(modName, actualModList[modDirectory]);
    }

    [TestMethod]
    [DoNotParallelize]
    public void TestGetModPath()
    {
        var tempDirectory = this.CreateResultsTempDirectory();

        var configDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer)));
        var penumbraConfigDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(Penumbra)));

        Directory.CreateDirectory(Path.Combine(penumbraConfigDirectory.FullName, "mod_data"));

        var modsDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, "ModsDirectory"));

        var modInterop = new ModInteropBuilder()
            .WithPluginLogDefaults()
            .WithPenumbraApiModMovedNoop()
            .WithPenumbraApiGetModList([])
            .WithPenumbraApiGetChangedItems([])
            .WithPenumbraApiModAddedOrDeletedNoop()
            .WithPenumbraApiModDirectoryChangedNoop()
            .WithPenumbraApiGetModDirectory(modsDirectory)
            .WithPenumbraApiSetModPath(PenumbraApiEc.Success)
            .WithPluginInterfaceInjectObject(false)
            .WithPluginInterfaceConfigDirectory(configDirectory)
            .Build();

        var modDirectory = "Mod Directory";
        var modPath = "Mod Path";

        var sortOrder = new SortOrder() { Data = { { modDirectory, modPath } } };

        using var _ = new ModInteropShimsContextBuilder()
            .WithFileReaderTryReadFromFile(sortOrder)
            .Build();

        Assert.AreEqual(modPath, modInterop.GetModPath(modDirectory));
    }

    [TestMethod]
    [DoNotParallelize]
    public void TestGetModPathWithoutSortOrder()
    {
        var tempDirectory = this.CreateResultsTempDirectory();

        var configDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer)));
        var penumbraConfigDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(Penumbra)));

        Directory.CreateDirectory(Path.Combine(penumbraConfigDirectory.FullName, "mod_data"));

        var modsDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, "ModsDirectory"));

        var modInterop = new ModInteropBuilder()
            .WithPluginLogDefaults()
            .WithPenumbraApiModMovedNoop()
            .WithPenumbraApiGetModList([])
            .WithPenumbraApiGetChangedItems([])
            .WithPenumbraApiModAddedOrDeletedNoop()
            .WithPenumbraApiModDirectoryChangedNoop()
            .WithPenumbraApiGetModDirectory(modsDirectory)
            .WithPenumbraApiSetModPath(PenumbraApiEc.Success)
            .WithPluginInterfaceInjectObject(false)
            .WithPluginInterfaceConfigDirectory(configDirectory)
            .Build();

        using var _ = new ModInteropShimsContextBuilder()
            .WithFileReaderTryReadFromFile(null as SortOrder)
            .Build();

        var modDirectory = "Mod Directory";
        var modPath = modInterop.GetModPath(modDirectory);

        Assert.AreEqual(modDirectory, modPath);
    }

    [TestMethod]
    [DoNotParallelize]
    public void TestGetModDirectory()
    {
        var tempDirectory = this.CreateResultsTempDirectory();

        var configDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer)));
        var penumbraConfigDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(Penumbra)));

        Directory.CreateDirectory(Path.Combine(penumbraConfigDirectory.FullName, "mod_data"));

        var modsDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, "ModsDirectory"));

        var modInterop = new ModInteropBuilder()
            .WithPluginLogDefaults()
            .WithPenumbraApiModMovedNoop()
            .WithPenumbraApiGetModList([])
            .WithPenumbraApiGetChangedItems([])
            .WithPenumbraApiModAddedOrDeletedNoop()
            .WithPenumbraApiModDirectoryChangedNoop()
            .WithPenumbraApiGetModDirectory(modsDirectory)
            .WithPenumbraApiSetModPath(PenumbraApiEc.Success)
            .WithPluginInterfaceInjectObject(false)
            .WithPluginInterfaceConfigDirectory(configDirectory)
            .Build();

        var modDirectory = "Mod Directory";
        var modPath = "Mod Path";

        var sortOrder = new SortOrder() { Data = { { modDirectory, modPath } } };

        using var _ = new ModInteropShimsContextBuilder()
            .WithFileReaderTryReadFromFile(sortOrder)
            .Build();

        Assert.AreEqual(modDirectory, modInterop.GetModDirectory(modPath));
    }

    [TestMethod]
    [DoNotParallelize]
    public void TestGetModDirectoryWithoutSortOrder()
    {
        var tempDirectory = this.CreateResultsTempDirectory();

        var configDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer)));
        var penumbraConfigDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(Penumbra)));

        Directory.CreateDirectory(Path.Combine(penumbraConfigDirectory.FullName, "mod_data"));

        var modsDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, "ModsDirectory"));

        var modInterop = new ModInteropBuilder()
            .WithPluginLogDefaults()
            .WithPenumbraApiModMovedNoop()
            .WithPenumbraApiGetModList([])
            .WithPenumbraApiGetChangedItems([])
            .WithPenumbraApiModAddedOrDeletedNoop()
            .WithPenumbraApiModDirectoryChangedNoop()
            .WithPenumbraApiGetModDirectory(modsDirectory)
            .WithPenumbraApiSetModPath(PenumbraApiEc.Success)
            .WithPluginInterfaceInjectObject(false)
            .WithPluginInterfaceConfigDirectory(configDirectory)
            .Build();

        using var _ = new ModInteropShimsContextBuilder()
            .WithFileReaderTryReadFromFile(null as SortOrder)
            .Build();

        var modPath = "Mod Path";
        var actualModPath = modInterop.GetModDirectory(modPath);

        Assert.AreEqual(modPath, actualModPath);
    }

    [TestMethod]
    public void TestSetModPath()
    {
        var tempDirectory = this.CreateResultsTempDirectory();

        var observer = new StubObserver();

        var configDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer)));
        var penumbraConfigDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(Penumbra)));

        Directory.CreateDirectory(Path.Combine(penumbraConfigDirectory.FullName, "mod_data"));

        var modsDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, "ModsDirectory"));

        var setModPathObserver = new StubObserver();
        var setModPath = StubFunc.WithObserver(setModPathObserver, (string modDirectory, string modName, string newModPath) => PenumbraApiEc.Success);
        
        var modInterop = new ModInteropBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithPenumbraApiModMovedNoop()
            .WithPenumbraApiGetModList([])
            .WithPenumbraApiGetChangedItems([])
            .WithPenumbraApiModAddedOrDeletedNoop()
            .WithPenumbraApiModDirectoryChangedNoop()
            .WithPenumbraApiGetModDirectory(modsDirectory)
            .WithPenumbraApiSetModPath(setModPath)
            .WithPluginInterfaceInjectObject(false)
            .WithPluginInterfaceConfigDirectory(configDirectory)
            .Build();

        var beforeLogCalls = observer.GetCalls();
        Assert.HasCount(1, beforeLogCalls);

        AssertPluginLog.MatchObservedCall(beforeLogCalls[0], nameof(IPluginLog.Debug), 
            actualMessage => Assert.AreEqual("Created mod file system watchers", actualMessage));

        var modDirectory = "Mod Directory";
        var newModPath = "New Mod Path";
        
        var success = modInterop.SetModPath(modDirectory, newModPath);

        var setModPathCalls = setModPathObserver.GetCalls();
        Assert.HasCount(1, setModPathCalls);

        var setModPathArguments = setModPathCalls[0].GetArguments();
        Assert.AreEqual(modDirectory, setModPathArguments[0] as string);
        Assert.AreEqual(string.Empty, setModPathArguments[1] as string);
        Assert.AreEqual(newModPath, setModPathArguments[2] as string);

        var afterLogCalls = observer.GetCalls();
        Assert.HasCount(5, afterLogCalls);

        AssertPluginLog.MatchObservedCall(afterLogCalls[1], nameof(IPluginLog.Info), 
            actualMessage => Assert.AreEqual($"Set mod [{modDirectory}] path to [{newModPath}]", actualMessage));
        AssertPluginLog.MatchObservedCall(afterLogCalls[2], nameof(IPluginLog.Debug), 
            actualMessage => Assert.AreEqual($"Invalidating caches for mod [{modDirectory}]", actualMessage));
        AssertPluginLog.MatchObservedCall(afterLogCalls[3], nameof(IPluginLog.Debug), 
            actualMessage => Assert.AreEqual("Invalidating sort order data cache", actualMessage));
        AssertPluginLog.MatchObservedCall(afterLogCalls[4], nameof(IPluginLog.Debug), 
            actualMessage => Assert.AreEqual($"Invalidating mod info cache [{modDirectory}]", actualMessage));
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void TestReloadPenumbra(bool success)
    {
        var tempDirectory = this.CreateResultsTempDirectory();

        var observer = new StubObserver();

        var configDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer)));
        var penumbraConfigDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(Penumbra)));

        Directory.CreateDirectory(Path.Combine(penumbraConfigDirectory.FullName, "mod_data"));

        var modsDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, "ModsDirectory"));

        var modInterop = new ModInteropBuilder()
            .WithPluginLogDefaults()
            .WithCommandManagerObserver(observer)
            .WithCommandManagerProcessCommand(success)
            .WithPenumbraApiModMovedNoop()
            .WithPenumbraApiGetModList([])
            .WithPenumbraApiGetChangedItems([])
            .WithPenumbraApiModAddedOrDeletedNoop()
            .WithPenumbraApiModDirectoryChangedNoop()
            .WithPenumbraApiGetModDirectory(modsDirectory)
            .WithPenumbraApiSetModPath(PenumbraApiEc.Success)
            .WithPluginInterfaceInjectObject(false)
            .WithPluginInterfaceConfigDirectory(configDirectory)
            .Build();

        Assert.AreEqual(success, modInterop.ReloadPenumbra());

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        Assert.AreEqual(ModInterop.RELOAD_PENUMBRA_COMMAND, calls[0].GetArguments()[0] as string);
    }
}
