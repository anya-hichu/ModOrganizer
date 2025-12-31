using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.DefaultMods;
using ModOrganizer.Json.Penumbra.LocalModDatas;
using ModOrganizer.Json.Penumbra.ModMetas;
using ModOrganizer.Json.Penumbra.SortOrders;
using ModOrganizer.Tests.Json.Readers.Files;
using ModOrganizer.Tests.Dalamuds.PenumbraApis;
using ModOrganizer.Tests.Dalamuds.PluginInterfaces;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
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
        string? actualModDirectory = null;

        modInterop.OnModAdded += directory => actualModDirectory = directory;

        var beforeCalls = observer.GetCalls();
        Assert.HasCount(1, beforeCalls);
        AssertPluginLog.MatchObservedCall(beforeCalls[0], nameof(IPluginLog.Debug), 
            actualMessage => Assert.AreEqual("Created mod file system watchers", actualMessage));

        registeredAction.Invoke(modDirectory);

        Assert.AreEqual(modDirectory, actualModDirectory);

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
        string? actualModDirectory = null;

        modInterop.OnModDeleted += directory => actualModDirectory = directory;

        var beforeCalls = observer.GetCalls();
        Assert.HasCount(1, beforeCalls);
        AssertPluginLog.MatchObservedCall(beforeCalls[0], nameof(IPluginLog.Debug), 
            actualMessage => Assert.AreEqual("Created mod file system watchers", actualMessage));

        registeredAction.Invoke(modDirectory);

        Assert.AreEqual(modDirectory, actualModDirectory);

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

        string? actualModDirectory = null;
        string? actualNewModDirectory = null;

        modInterop.OnModMoved += (directory, newDirectory) =>
        {
            actualModDirectory = directory;
            actualNewModDirectory = newDirectory;
        };

        var beforeCalls = observer.GetCalls();
        Assert.HasCount(1, beforeCalls);
        AssertPluginLog.MatchObservedCall(beforeCalls[0], nameof(IPluginLog.Debug), 
            actualMessage => Assert.AreEqual("Created mod file system watchers", actualMessage));

        registeredAction.Invoke(modDirectory, newModDirectory);

        Assert.AreEqual(modDirectory, actualModDirectory);
        Assert.AreEqual(newModDirectory, actualNewModDirectory);

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

        var sortOrderChanged = false;
        modInterop.OnSortOrderChanged += () => sortOrderChanged = true;

        var beforeCalls = observer.GetCalls();
        Assert.HasCount(1, beforeCalls);
        AssertPluginLog.MatchObservedCall(beforeCalls[0], nameof(IPluginLog.Debug),
            actualMessage => Assert.AreEqual("Created mod file system watchers", actualMessage));

        var sortOrder = modInterop.GetSortOrder();

        Assert.IsTrue(sortOrderChanged);

        var afterCalls = observer.GetCalls();
        Assert.HasCount(4, afterCalls);

        AssertPluginLog.MatchObservedCall(afterCalls[1], nameof(IPluginLog.Error),
            actualMessage => Assert.StartsWith("Caught exception while reading [JsonElement]", actualMessage));
        AssertPluginLog.MatchObservedCall(afterCalls[2], nameof(IPluginLog.Warning),
            actualMessage => Assert.StartsWith("Failed to read [JsonElement] from json file", actualMessage));
        AssertPluginLog.MatchObservedCall(afterCalls[3], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual("Failed to parse [SortOrder], cached empty until next file system update or reload", actualMessage));
    }

    [TestMethod]
    public async Task TestToggleFsWatchers()
    {
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

        await Task.Delay(10, TestContext.CancellationToken);

        Assert.HasCount(11, observer.GetCalls());
    }

    [TestMethod]
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

        var sortOrder = new SortOrder() { Data = { { modDirectory, modPath } } };

        var localModData = new LocalModData() { FileVersion = 0 };
        var defaultMod = new DefaultMod();
        var modMeta = new ModMeta() { FileVersion = 0, Name = "Mod Name" };

        using var _ = new ModInteropShimsContextBuilder()
            .WithIReadableFileTryReadFromFile(sortOrder)
            .WithIReadableFileTryReadFromFile(localModData)
            .WithIReadableFileTryReadFromFile(defaultMod)
            .WithIReadableFileTryReadFromFile(modMeta)
            .Build();

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
        throw new NotImplementedException();
    }

    [TestMethod]
    public void TestGetModPath()
    {
        throw new NotImplementedException();
    }

    [TestMethod]
    public void TestGetModDirectory()
    {
        throw new NotImplementedException();
    }

    [TestMethod]
    public void TestSetModPath()
    {
        throw new NotImplementedException();
    }

    [TestMethod]
    public void TestReloadPenumbra()
    {
        throw new NotImplementedException();
    }
}
