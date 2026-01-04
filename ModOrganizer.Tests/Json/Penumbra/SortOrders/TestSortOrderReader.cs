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

        var jsonElement = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() 
        {
            {
                nameof(SortOrder.Data), new Dictionary<string, object?>() 
                { 
                    { modDirectory, modPath } 
                } 
            }
        });

        var success = new SortOrderReaderBuilder()
            .Build()
            .TryRead(jsonElement, out var sortOrder);

        Assert.IsTrue(success);
        Assert.IsNotNull(sortOrder);

        Assert.HasCount(1, sortOrder.Data);
        Assert.AreEqual(modPath, sortOrder.Data[modDirectory]);

        Assert.IsEmpty(sortOrder.EmptyFolders);
    }

    [TestMethod]
    public void TestTryReadWithInvalidKind()
    {
        var observer = new StubObserver();

        var jsonElement = JsonSerializer.SerializeToElement(null as object);

        var sortOrderReader = new SortOrderReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = sortOrderReader.TryRead(jsonElement, out var sortOrder);

        Assert.IsFalse(success);
        Assert.IsNull(sortOrder);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual("Expected value kind [Object] but got [Null]: ", actualMessage));
    }

    [TestMethod]
    public void TestTryReadWithInvalidData()
    {
        var observer = new StubObserver();

        var jsonElement = JsonSerializer.SerializeToElement(new Dictionary<string, object?>
        { 
            { nameof(SortOrder.Data), null } 
        });

        var sortOrderReader = new SortOrderReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = sortOrderReader.TryRead(jsonElement, out var sortOrder);

        Assert.IsFalse(success);
        Assert.IsNull(sortOrder);

        var calls = observer.GetCalls();
        Assert.HasCount(2, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual("Expected value kind [Object] but got [Null]: ", actualMessage));
        AssertPluginLog.MatchObservedCall(calls[1], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual("Failed to read one or more [Data] for [SortOrder]: ", actualMessage));
    }

    [TestMethod]
    public void TestTryReadWithInvalidEmptyFolders()
    {
        var observer = new StubObserver();

        var jsonElement = JsonSerializer.SerializeToElement(new Dictionary<string, object?> 
        { 
            { nameof(SortOrder.EmptyFolders), null } 
        });

        var sortOrderReader = new SortOrderReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = sortOrderReader.TryRead(jsonElement, out var sortOrder);

        Assert.IsFalse(success);
        Assert.IsNull(sortOrder);

        var calls = observer.GetCalls();
        Assert.HasCount(2, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual("Expected value kind [Array] but got [Null]: ", actualMessage));
        AssertPluginLog.MatchObservedCall(calls[1], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual("Failed to read one or more [EmptyFolders] for [SortOrder]: ", actualMessage));
    }

    [TestMethod]
    public void TestTryReadFromFile()
    {
        var observer = new StubObserver();

        var modDirectory = "Mod Directory";
        var modPath = "Mod Path";

        var jsonElement = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() 
        { 
            { 
                nameof(SortOrder.Data), new Dictionary<string, object?>() 
                { 
                    { modDirectory, modPath } 
                } 
            } 
        });

        var sortOrderReader = new SortOrderReaderBuilder()
            .WithElementReaderObserver(observer)
            .WithElementReaderTryReadFromFile(jsonElement)
            .Build();

        var filePath = "File Path";
        var success = sortOrderReader.TryReadFromFile(filePath, out var sortOrder);

        Assert.IsTrue(success);
        Assert.IsNotNull(sortOrder);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        var call = calls[0];
        Assert.AreEqual(nameof(IElementReader.TryReadFromFile), call.StubbedMethod.Name);
        Assert.AreEqual(filePath, call.GetArguments()[0] as string);

        Assert.HasCount(1, sortOrder.Data);
        Assert.AreEqual(modPath, sortOrder.Data[modDirectory]);

        Assert.IsEmpty(sortOrder.EmptyFolders);
    }

    [TestMethod]
    public void TestTryReadFromFileWithInvalid()
    {
        var observer = new StubObserver();

        var jsonElement = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() 
        { 
            { nameof(SortOrder.Data), null } 
        });

        var sortOrderReader = new SortOrderReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithElementReaderTryReadFromFile(jsonElement)
            .Build();

        var filePath = "File Path";
        var success = sortOrderReader.TryReadFromFile(filePath, out var sortOrder);

        Assert.IsFalse(success);
        Assert.IsNull(sortOrder);

        var calls = observer.GetCalls();
        Assert.HasCount(3, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), actualMessage => Assert.AreEqual("Expected value kind [Object] but got [Null]: ", actualMessage));
        AssertPluginLog.MatchObservedCall(calls[1], nameof(IPluginLog.Warning), actualMessage => Assert.AreEqual("Failed to read one or more [Data] for [SortOrder]: ", actualMessage));
        AssertPluginLog.MatchObservedCall(calls[2], nameof(IPluginLog.Warning), actualMessage => Assert.AreEqual($"Failed to read instance [SortOrder] from json file [{filePath}]", actualMessage));
    }
}
