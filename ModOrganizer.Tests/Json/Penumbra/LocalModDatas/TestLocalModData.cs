using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.LocalModDatas;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.Penumbra.LocalModDatas;

[TestClass]
public class TestLocalModData
{
    [TestMethod]
    public void TestTryRead()
    {
        var fileVersion = LocalModDataReader.SUPPORTED_FILE_VERSION;

        var importDate = 2;
        var localTag = "Local Tag";
        var preferredChangedItem = 1;

        var reader = new LocalModDataReaderBuilder().Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(LocalModDataV3.FileVersion), fileVersion },
            { nameof(LocalModDataV3.ImportDate), importDate },
            { nameof(LocalModDataV3.LocalTags), new string[] { localTag } },
            { nameof(LocalModDataV3.Favorite), true },
            { nameof(LocalModDataV3.PreferredChangedItems), new int[] { preferredChangedItem } }
        });

        var success = reader.TryRead(element, out var localModData);

        Assert.IsTrue(success);
        Assert.IsNotNull(localModData);

        Assert.AreEqual(fileVersion, localModData.FileVersion);
        Assert.AreEqual(importDate, localModData.ImportDate);

        Assert.IsNotNull(localModData.LocalTags);
        Assert.HasCount(1, localModData.LocalTags);
        Assert.AreEqual(localTag, localModData.LocalTags[0]);

        Assert.IsTrue(localModData.Favorite);

        Assert.IsNotNull(localModData.PreferredChangedItems);
        Assert.HasCount(1, localModData.PreferredChangedItems);
        Assert.AreEqual(preferredChangedItem, localModData.PreferredChangedItems[0]);
    }

    [TestMethod]
    public void TestTryReadWithDefaults()
    {
        var fileVersion = LocalModDataReader.SUPPORTED_FILE_VERSION;

        var reader = new LocalModDataReaderBuilder().Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(LocalModDataV3.FileVersion), fileVersion }
        });

        var success = reader.TryRead(element, out var localModData);

        Assert.IsTrue(success);
        Assert.IsNotNull(localModData);

        Assert.AreEqual(fileVersion, localModData.FileVersion);

        Assert.IsNull(localModData.ImportDate);
        Assert.IsNull(localModData.LocalTags);
        Assert.IsNull(localModData.Favorite);
        Assert.IsNull(localModData.PreferredChangedItems);
    }

    [TestMethod]
    public void TestTryReadWithInvalidKind()
    {
        var observer = new StubObserver();

        var reader = new LocalModDataReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var element = JsonSerializer.SerializeToElement(null as object);

        var success = reader.TryRead(element, out var localModData);

        Assert.IsFalse(success);
        Assert.IsNull(localModData);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [Object] value kind but found [Null]: ", actualMessage));
    }

    [TestMethod]
    public void TestTryReadWithoutVersion()
    {
        var observer = new StubObserver();

        var reader = new LocalModDataReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>());

        var success = reader.TryRead(element, out var localModData);

        Assert.IsFalse(success);
        Assert.IsNull(localModData);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected property [FileVersion] to be present: {element}", actualMessage));
    }

    [TestMethod]
    public void TestTryReadWithInvalidVersion()
    {
        var observer = new StubObserver();

        var fileVersion = 0u;

        var reader = new LocalModDataReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(LocalModDataV3.FileVersion), fileVersion }
        });

        var success = reader.TryRead(element, out var localModData);

        Assert.IsFalse(success);
        Assert.IsNull(localModData);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Failed to read [LocalModDataV3], unsupported [FileVersion] found [{fileVersion}] (supported version: 3): {element}", actualMessage));
    }

    [TestMethod]
    [DataRow(nameof(LocalModDataV3.ImportDate), JsonValueKind.Number)]
    [DataRow(nameof(LocalModDataV3.LocalTags), JsonValueKind.Array)]
    [DataRow(nameof(LocalModDataV3.PreferredChangedItems), JsonValueKind.Array)]
    public void TestTryReadWithInvalidValueKind(string propertyName, JsonValueKind kind)
    {
        var observer = new StubObserver();

        var fileVersion = LocalModDataReader.SUPPORTED_FILE_VERSION;

        var reader = new LocalModDataReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var propertyValue = false;

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(LocalModDataV3.FileVersion), fileVersion },
            { propertyName, propertyValue }
        });

        var success = reader.TryRead(element, out var localModData);

        Assert.IsFalse(success);
        Assert.IsNull(localModData);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
           actualMessage => Assert.AreEqual($"Expected [{kind}] value kind but found [False]: {propertyValue}", actualMessage));
    }

    [TestMethod]
    public void TestTryReadWithInvalidFavoriteKind()
    {
        var observer = new StubObserver();

        var fileVersion = LocalModDataReader.SUPPORTED_FILE_VERSION;

        var reader = new LocalModDataReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var propertyValue = 0;

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(LocalModDataV3.FileVersion), fileVersion },
            { nameof(LocalModDataV3.Favorite), propertyValue }
        });

        var success = reader.TryRead(element, out var localModData);

        Assert.IsFalse(success);
        Assert.IsNull(localModData);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
           actualMessage => Assert.AreEqual($"Expected [Number] value kind to be parsable as [Boolean]: {propertyValue}", actualMessage));
    }
}
