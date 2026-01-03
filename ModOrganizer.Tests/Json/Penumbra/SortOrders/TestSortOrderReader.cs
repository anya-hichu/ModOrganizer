using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.SortOrders;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.Penumbra.SortOrders;

[TestClass]
public class TestSortOrderReader
{
    [TestMethod]
    public void TestTryRead()
    {
        var sortOrderReader = new SortOrderReaderBuilder().Build();

        var modDirectory = "Mod Directory";
        var modPath = "Mod Path";

        var jsonElement = JsonSerializer.SerializeToElement(new SortOrder() { Data = new() { { modDirectory, modPath } } });

        var success = sortOrderReader.TryRead(jsonElement, out var actualSortOrder);

        Assert.IsTrue(success);
        Assert.IsNotNull(actualSortOrder);

        Assert.HasCount(1, actualSortOrder.Data);
        Assert.AreEqual(modPath, actualSortOrder.Data[modDirectory]);

        Assert.IsEmpty(actualSortOrder.EmptyFolders);
    }

    [TestMethod]
    public void TestTryReadWithInvalidKind()
    {
        var observer = new StubObserver();

        var sortOrderReader = new SortOrderReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var jsonElement = JsonSerializer.SerializeToElement(null as object);

        var success = sortOrderReader.TryRead(jsonElement, out var actualSortOrder);

        Assert.IsFalse(success);
        Assert.IsNull(actualSortOrder);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), actualMessage => Assert.AreEqual("Expected value kind [Object] but got [Null]: ", actualMessage));
    }

    [TestMethod]
    public void TestTryReadWithInvalidData()
    {
        var observer = new StubObserver();

        var sortOrderReader = new SortOrderReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var jsonElement = JsonSerializer.SerializeToElement(new Dictionary<string, object?> { { nameof(SortOrder.Data), null } });

        var success = sortOrderReader.TryRead(jsonElement, out var actualSortOrder);

        Assert.IsFalse(success);
        Assert.IsNull(actualSortOrder);

        var calls = observer.GetCalls();
        Assert.HasCount(2, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), actualMessage => Assert.AreEqual("Expected value kind [Object] but got [Null]: ", actualMessage));
        AssertPluginLog.MatchObservedCall(calls[1], nameof(IPluginLog.Warning), actualMessage => Assert.AreEqual("Failed to read one or more [Data] for [SortOrder]: ", actualMessage));
    }

    [TestMethod]
    public void TestTryReadWithInvalidEmptyFolders()
    {
        var observer = new StubObserver();

        var sortOrderReader = new SortOrderReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var jsonElement = JsonSerializer.SerializeToElement(new Dictionary<string, object?> { { nameof(SortOrder.EmptyFolders), null } });

        var success = sortOrderReader.TryRead(jsonElement, out var actualSortOrder);

        Assert.IsFalse(success);
        Assert.IsNull(actualSortOrder);

        var calls = observer.GetCalls();
        Assert.HasCount(2, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), actualMessage => Assert.AreEqual("Expected value kind [Array] but got [Null]: ", actualMessage));
        AssertPluginLog.MatchObservedCall(calls[1], nameof(IPluginLog.Warning), actualMessage => Assert.AreEqual("Failed to read one or more [EmptyFolders] for [SortOrder]: ", actualMessage));
    }
}
