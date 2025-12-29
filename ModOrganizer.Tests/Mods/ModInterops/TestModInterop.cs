using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Tests.Shared.PenumbraApis;
using ModOrganizer.Tests.Shared.PluginInterfaces;
using ModOrganizer.Tests.Shared.PluginLogs;
using Penumbra.Api.Enums;

namespace ModOrganizer.Tests.Mods.ModInterops;

[TestClass]
public class TestModInterop : TestClass
{
    [TestMethod]
    public void TestOnModAdded()
    {
        var observer = new StubObserver();

        var tempDirectory = CreateResultsTempDirectory();

        var configDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer)));
        var penumbraConfigDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(Penumbra)));

        Directory.CreateDirectory(Path.Combine(penumbraConfigDirectory.FullName, "mod_data"));

        var modsDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, "Mods"));

        var actions = new HashSet<Action<string>>();

        var modInterop = new ModInteropBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithPenumbraApiModAdded(actions)
            .WithPenumbraApiModMovedNoop()
            .WithPenumbraApiModDirectoryChangedNoop()
            .WithPenumbraApiGetModList([])
            .WithPenumbraApiGetChangedItems([])
            .WithPenumbraApiGetModDirectory(modsDirectory)
            .WithPluginInterfaceInjectObject(false)
            .WithPenumbraApiSetModPath(PenumbraApiEc.Success)
            .WithPluginInterfaceConfigDirectory(configDirectory)
            .Build();

        Assert.HasCount(1, actions);
        var action = actions.ElementAt(0);

        var modDirectory = "Mod Directory";
        string? actualModDirectory = null;

        modInterop.OnModAdded += directory => actualModDirectory = directory;

        var beforeCalls = observer.GetCalls();
        Assert.HasCount(1, beforeCalls);
        AssertPluginLog.MatchObservedCall(beforeCalls[0], nameof(IPluginLog.Debug), actualMessage => Assert.AreEqual("Created mod file system watchers", actualMessage));

        action?.Invoke(modDirectory);

        Assert.AreEqual(modDirectory, actualModDirectory);

        var afterCalls = observer.GetCalls();
        Assert.HasCount(5, afterCalls);
        AssertPluginLog.MatchObservedCall(afterCalls[1], nameof(IPluginLog.Debug), actualMessage => Assert.AreEqual($"Received mod added event [{modDirectory}]", actualMessage));
        AssertPluginLog.MatchObservedCall(afterCalls[2], nameof(IPluginLog.Debug), actualMessage => Assert.AreEqual($"Invalidating caches for mod [{modDirectory}]", actualMessage));
        AssertPluginLog.MatchObservedCall(afterCalls[3], nameof(IPluginLog.Debug), actualMessage => Assert.AreEqual($"Invalidating sort order data cache", actualMessage));
        AssertPluginLog.MatchObservedCall(afterCalls[4], nameof(IPluginLog.Debug), actualMessage => Assert.AreEqual($"Invalidating mod info cache [{modDirectory}]", actualMessage));
    }

    [TestMethod]
    public void TestOnModDeleted()
    {
        var observer = new StubObserver();

        var tempDirectory = CreateResultsTempDirectory();

        var configDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer)));
        var penumbraConfigDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(Penumbra)));

        Directory.CreateDirectory(Path.Combine(penumbraConfigDirectory.FullName, "mod_data"));

        var modsDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, "Mods"));

        var actions = new HashSet<Action<string>>();

        var modInterop = new ModInteropBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithPenumbraApiModDeleted(actions)
            .WithPenumbraApiModMovedNoop()
            .WithPenumbraApiModDirectoryChangedNoop()
            .WithPenumbraApiGetModList([])
            .WithPenumbraApiGetChangedItems([])
            .WithPenumbraApiGetModDirectory(modsDirectory)
            .WithPenumbraApiSetModPath(PenumbraApiEc.Success)
            .WithPluginInterfaceInjectObject(false)
            .WithPluginInterfaceConfigDirectory(configDirectory)
            .Build();

        Assert.HasCount(1, actions);
        var action = actions.ElementAt(0);

        var modDirectory = "Mod Directory";
        string? actualModDirectory = null;

        modInterop.OnModDeleted += directory => actualModDirectory = directory;

        var beforeCalls = observer.GetCalls();
        Assert.HasCount(1, beforeCalls);
        AssertPluginLog.MatchObservedCall(beforeCalls[0], nameof(IPluginLog.Debug), actualMessage => Assert.AreEqual("Created mod file system watchers", actualMessage));

        action?.Invoke(modDirectory);

        Assert.AreEqual(modDirectory, actualModDirectory);

        var afterCalls = observer.GetCalls();
        Assert.HasCount(5, afterCalls);
        AssertPluginLog.MatchObservedCall(afterCalls[1], nameof(IPluginLog.Debug), actualMessage => Assert.AreEqual($"Received mod deleted event [{modDirectory}]", actualMessage));
        AssertPluginLog.MatchObservedCall(afterCalls[2], nameof(IPluginLog.Debug), actualMessage => Assert.AreEqual($"Invalidating caches for mod [{modDirectory}]", actualMessage));
        AssertPluginLog.MatchObservedCall(afterCalls[3], nameof(IPluginLog.Debug), actualMessage => Assert.AreEqual($"Invalidating sort order data cache", actualMessage));
        AssertPluginLog.MatchObservedCall(afterCalls[4], nameof(IPluginLog.Debug), actualMessage => Assert.AreEqual($"Invalidating mod info cache [{modDirectory}]", actualMessage));
    }

    [TestMethod]
    public void TestOnModMoved()
    {
        var observer = new StubObserver();

        var tempDirectory = CreateResultsTempDirectory();

        var configDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer)));
        var penumbraConfigDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(Penumbra)));

        Directory.CreateDirectory(Path.Combine(penumbraConfigDirectory.FullName, "mod_data"));

        var modsDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, "Mods"));

        var actions = new HashSet<Action<string, string>>();

        var modInterop = new ModInteropBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithPenumbraApiModMoved(actions)
            .WithPenumbraApiModAddedOrDeletedNoop()
            .WithPenumbraApiModDirectoryChangedNoop()
            .WithPenumbraApiGetModList([])
            .WithPenumbraApiGetChangedItems([])
            .WithPenumbraApiGetModDirectory(modsDirectory)
            .WithPluginInterfaceInjectObject(false)
            .WithPenumbraApiSetModPath(PenumbraApiEc.Success)
            .WithPluginInterfaceConfigDirectory(configDirectory)
            .Build();

        Assert.HasCount(1, actions);
        var action = actions.ElementAt(0);

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
        AssertPluginLog.MatchObservedCall(beforeCalls[0], nameof(IPluginLog.Debug), actualMessage => Assert.AreEqual("Created mod file system watchers", actualMessage));

        action?.Invoke(modDirectory, newModDirectory);

        Assert.AreEqual(modDirectory, actualModDirectory);
        Assert.AreEqual(newModDirectory, actualNewModDirectory);

        var afterCalls = observer.GetCalls();
        Assert.HasCount(5, afterCalls);
        AssertPluginLog.MatchObservedCall(afterCalls[1], nameof(IPluginLog.Debug), actualMessage => Assert.AreEqual($"Received mod moved event [{modDirectory}] to [{newModDirectory}]", actualMessage));
        AssertPluginLog.MatchObservedCall(afterCalls[2], nameof(IPluginLog.Debug), actualMessage => Assert.AreEqual($"Invalidating caches for mod [{modDirectory}]", actualMessage));
        AssertPluginLog.MatchObservedCall(afterCalls[3], nameof(IPluginLog.Debug), actualMessage => Assert.AreEqual($"Invalidating sort order data cache", actualMessage));
        AssertPluginLog.MatchObservedCall(afterCalls[4], nameof(IPluginLog.Debug), actualMessage => Assert.AreEqual($"Invalidating mod info cache [{modDirectory}]", actualMessage));
    }

    [TestMethod]
    public void TestOnModDirectoryChanged()
    {

    }

    [TestMethod]
    public void TestOnModsChanged()
    {


    }

    [TestMethod]
    public void TestOnSortOrderChanged()
    {

    }

    [TestMethod]
    public void TestToggleFsWatchers()
    {

    }

    [TestMethod]
    public void TestGetSortOrderPath()
    {

    }

    [TestMethod]
    public void TestGetSortOrder()
    {

    }

    [TestMethod]
    public void TestTryGetModInfo()
    {

    }

    [TestMethod]
    public void TestGetModList()
    {

    }

    [TestMethod]
    public void TestGetModPath()
    {

    }

    [TestMethod]
    public void TestGetModDirectory()
    {

    }

    [TestMethod]
    public void TestSetModPath()
    {

    }

    [TestMethod]
    public void TestReloadPenumbra()
    {

    }
}
