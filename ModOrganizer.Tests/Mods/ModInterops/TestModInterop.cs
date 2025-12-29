using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Tests.Shared.PenumbraApi;
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
        action?.Invoke(modDirectory);

        Assert.AreEqual(modDirectory, actualModDirectory);

        var calls = observer.GetCalls();
        Assert.HasCount(5, calls);

        var firstCall = calls[0];
        Assert.AreEqual(nameof(IPluginLog.Debug), firstCall.StubbedMethod.Name);

        var firstArguments = firstCall.GetArguments();
        Assert.HasCount(2, firstArguments);
        Assert.AreEqual("Created mod file system watchers", firstArguments[0] as string);
    }

    [TestMethod]
    public void TestOnModDeleted()
    {

    }

    [TestMethod]
    public void TestOnModMoved()
    {

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
