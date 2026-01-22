using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.SortOrders;
using ModOrganizer.Json.Readers.Elements;
using ModOrganizer.Json.Readers.Files;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Json.Readers.Elements;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.Penumbra.SortOrders;

[TestClass]
public class TestSortOrderReader
{
    [TestMethod]
    public void TestTryRead()
    {
        var modDirectory = "Mod Directory";
        var modPath = "Mod Path";

        var emptyFolder = "Empty Folder";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() 
        {
            { nameof(SortOrder.Data), new Dictionary<string, string>() { { modDirectory, modPath } } },
            { nameof(SortOrder.EmptyFolders), new string[] { emptyFolder } }
        });

        var sortOrderReader = new SortOrderReaderBuilder().Build();

        var success = sortOrderReader.TryRead(element, out var sortOrder);

        Assert.IsTrue(success);
        Assert.IsNotNull(sortOrder);

        Assert.HasCount(1, sortOrder.Data);
        Assert.AreEqual(modPath, sortOrder.Data[modDirectory]);

        Assert.HasCount(1, sortOrder.EmptyFolders);
        Assert.AreEqual(emptyFolder, sortOrder.EmptyFolders[0]);
    }

    [TestMethod]
    public void TestTryReadWithInvalidKind()
    {
        var observer = new StubObserver();

        var element = JsonSerializer.SerializeToElement(null as object);

        var sortOrderReader = new SortOrderReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = sortOrderReader.TryRead(element, out var sortOrder);

        Assert.IsFalse(success);
        Assert.IsNull(sortOrder);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual("Expected [Object] value kind but found [Null]: ", actualMessage));
    }

    [TestMethod]
    public void TestTryReadWithInvalidData()
    {
        var observer = new StubObserver();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>
        { 
            { nameof(SortOrder.Data), null } 
        });

        var sortOrderReader = new SortOrderReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = sortOrderReader.TryRead(element, out var sortOrder);

        Assert.IsFalse(success);
        Assert.IsNull(sortOrder);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected property [Data] to be present: {element}", actualMessage));
    }

    [TestMethod]
    public void TestTryReadWithInvalidEmptyFolders()
    {
        var observer = new StubObserver();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?> 
        {
            { nameof(SortOrder.Data), new Dictionary<string, string>() },
            { nameof(SortOrder.EmptyFolders), null } 
        });

        var sortOrderReader = new SortOrderReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = sortOrderReader.TryRead(element, out var sortOrder);

        Assert.IsFalse(success);
        Assert.IsNull(sortOrder);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected property [EmptyFolders] to be present: {element}", actualMessage));
    }

    [TestMethod]
    public void TestTryReadFromFile()
    {
        var observer = new StubObserver();

        var modDirectory = "Mod Directory";
        var modPath = "Mod Path";

        var emptyFolder = "Empty Folder";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(SortOrder.Data), new Dictionary<string, string>() { { modDirectory, modPath } } },
            { nameof(SortOrder.EmptyFolders), new string[] { emptyFolder } }
        });

        var sortOrderReader = new SortOrderReaderBuilder()
            .WithElementReaderObserver(observer)
            .WithElementReaderTryReadFromFile(element)
            .Build();

        var filePath = "File Path";
        var success = sortOrderReader.TryReadFromFile(filePath, out var sortOrder);

        Assert.IsTrue(success);
        Assert.IsNotNull(sortOrder);

        Assert.HasCount(1, sortOrder.Data);
        Assert.AreEqual(modPath, sortOrder.Data[modDirectory]);

        Assert.HasCount(1, sortOrder.EmptyFolders);
        Assert.AreEqual(emptyFolder, sortOrder.EmptyFolders[0]);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        var call = calls[0];
        Assert.AreEqual(nameof(IElementReader.TryReadFromFile), call.StubbedMethod.Name);
        Assert.AreEqual(filePath, call.GetArguments()[0] as string);
    }

    [TestMethod]
    public void TestTryReadFromFileWithInvalid()
    {
        var observer = new StubObserver();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { nameof(SortOrder.Data), null } });

        var sortOrderReader = new SortOrderReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithElementReaderTryReadFromFile(element)
            .Build();

        var filePath = "File Path";
        var success = sortOrderReader.TryReadFromFile(filePath, out var sortOrder);

        Assert.IsFalse(success);
        Assert.IsNull(sortOrder);

        var calls = observer.GetCalls();
        Assert.HasCount(2, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected property [Data] to be present: {element}", actualMessage));
        AssertPluginLog.MatchObservedCall(calls[1], nameof(IPluginLog.Warning), 
            actualMessage => Assert.AreEqual($"Failed to read instance [SortOrder] from json file [{filePath}]", actualMessage));
    }
}
