using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.SortOrders;
using ModOrganizer.Json.Readers.Elements;
using ModOrganizer.Json.Readers.Files;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Json.Asserts;
using ModOrganizer.Tests.Json.Readers.Elements;
using System.Text.Json;

using IAssert = ModOrganizer.Json.Asserts.IAssert;

namespace ModOrganizer.Tests.Json.Penumbra.SortOrders;

[TestClass]
public class TestSortOrderReader
{
    [TestMethod]
    public void TestTryRead()
    {
        var data = new Dictionary<string, string>();
        var emptyFolders = Array.Empty<string>();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() 
        {
            { nameof(SortOrder.Data), data },
            { nameof(SortOrder.EmptyFolders), emptyFolders }
        });

        var sortOrderReader = new SortOrderReaderBuilder()
            .WithAssertIsValue(true)
            .WithAssertIsStringDict(data)
            .WithAssertIsStringArray(emptyFolders)
            .Build();

        var success = sortOrderReader.TryRead(element, out var sortOrder);

        Assert.IsTrue(success);
        Assert.IsNotNull(sortOrder);

        Assert.AreSame(data, sortOrder.Data);
        Assert.AreSame(emptyFolders, sortOrder.EmptyFolders);
    }

    [TestMethod]
    public void TestTryReadWithInvalidKind()
    {
        var observer = new StubObserver();

        var element = JsonSerializer.SerializeToElement(null as object);

        var sortOrderReader = new SortOrderReaderBuilder()
            .WithAssertObserver(observer)
            .WithAssertIsValue(false)
            .Build();

        var success = sortOrderReader.TryRead(element, out var sortOrder);

        Assert.IsFalse(success);
        Assert.IsNull(sortOrder);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        var call = calls[0];
        Assert.AreEqual(nameof(IAssert.IsValue), call.StubbedMethod.Name);


        switch (call.GetArguments()[0])
        {
            case JsonElement actualElement:
                Assert.AreEqual(element, actualElement);
                break;
            default:
                Assert.Fail("Expected first call argument to be a JsonElement");
                break;
        }
    }

    [TestMethod]
    public void TestTryReadWithInvalidData()
    {
        var observer = new StubObserver();

        Dictionary<string, string>? data = null;

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>
        { 
            { nameof(SortOrder.Data), data } 
        });

        var sortOrderReader = new SortOrderReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithAssertIsValue(true)
            .WithAssertIsStringDict(data)
            .Build();

        var success = sortOrderReader.TryRead(element, out var sortOrder);

        Assert.IsFalse(success);
        Assert.IsNull(sortOrder);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Failed to read one or more [Data] for [SortOrder]: {element}", actualMessage));
    }

    [TestMethod]
    public void TestTryReadWithInvalidEmptyFolders()
    {
        var observer = new StubObserver();

        string[]? emptyFolders = null;

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?> 
        { 
            { nameof(SortOrder.EmptyFolders), emptyFolders } 
        });

        var sortOrderReader = new SortOrderReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithAssertIsValue(true)
            .WithAssertIsStringArray(emptyFolders)
            .Build();

        var success = sortOrderReader.TryRead(element, out var sortOrder);

        Assert.IsFalse(success);
        Assert.IsNull(sortOrder);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Failed to read one or more [EmptyFolders] for [SortOrder]: {element}", actualMessage));
    }

    [TestMethod]
    public void TestTryReadFromFile()
    {
        var observer = new StubObserver();

        var data = new Dictionary<string, string>();
        var emptyFolders = Array.Empty<string>();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(SortOrder.Data), data },
            { nameof(SortOrder.EmptyFolders), emptyFolders }
        });

        var sortOrderReader = new SortOrderReaderBuilder()
            .WithAssertIsValue(true)
            .WithAssertIsStringDict(data)
            .WithAssertIsStringArray(emptyFolders)
            .WithElementReaderObserver(observer)
            .WithElementReaderTryReadFromFile(element)
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

        Assert.AreSame(data, sortOrder.Data);
        Assert.AreSame(emptyFolders, sortOrder.EmptyFolders);
    }

    [TestMethod]
    public void TestTryReadFromFileWithInvalid()
    {
        var observer = new StubObserver();

        var data = Array.Empty<string>();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() 
        { 
            { nameof(SortOrder.Data), data } 
        });

        var sortOrderReader = new SortOrderReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithAssertIsValue(true)
            .WithAssertIsStringDict(null as Dictionary<string, string>)
            .WithElementReaderTryReadFromFile(element)
            .Build();

        var filePath = "File Path";
        var success = sortOrderReader.TryReadFromFile(filePath, out var sortOrder);

        Assert.IsFalse(success);
        Assert.IsNull(sortOrder);

        var calls = observer.GetCalls();
        Assert.HasCount(2, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), actualMessage => Assert.AreEqual($"Failed to read one or more [Data] for [SortOrder]: {element}", actualMessage));
        AssertPluginLog.MatchObservedCall(calls[1], nameof(IPluginLog.Warning), actualMessage => Assert.AreEqual($"Failed to read instance [SortOrder] from json file [{filePath}]", actualMessage));
    }
}
