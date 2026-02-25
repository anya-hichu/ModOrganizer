using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Atchs.Entries;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.Penumbra.Manipulations.Metas.Atchs.Entries;

[TestClass]
public class TestMetaAtchEntryReader
{
    [TestMethod]
    public void TestTryRead()
    {
        var bone = "Bone";
        var scale = 1;
        var offsetX = 2;
        var offsetY = 3;
        var offsetZ = 4;
        var rotationX = 5;
        var rotationY = 6;
        var rotationZ = 7;

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>
        {
            { nameof(MetaAtchEntry.Bone), bone },
            { nameof(MetaAtchEntry.Scale), scale },
            { nameof(MetaAtchEntry.OffsetX), offsetX },
            { nameof(MetaAtchEntry.OffsetY), offsetY },
            { nameof(MetaAtchEntry.OffsetZ), offsetZ },
            { nameof(MetaAtchEntry.RotationX), rotationX },
            { nameof(MetaAtchEntry.RotationY), rotationY },
            { nameof(MetaAtchEntry.RotationZ), rotationZ },
        });

        var reader = new MetaAtchEntryReaderBuilder().Build();

        var success = reader.TryRead(element, out var metaAtchEntry);

        Assert.IsTrue(success);
        Assert.IsNotNull(metaAtchEntry);

        Assert.AreEqual(metaAtchEntry.Bone, bone);
        Assert.AreEqual(metaAtchEntry.Scale, scale);
        Assert.AreEqual(metaAtchEntry.OffsetX, offsetX);
        Assert.AreEqual(metaAtchEntry.OffsetY, offsetY);
        Assert.AreEqual(metaAtchEntry.OffsetZ, offsetZ);
        Assert.AreEqual(metaAtchEntry.RotationX, rotationX);
        Assert.AreEqual(metaAtchEntry.RotationY, rotationY);
        Assert.AreEqual(metaAtchEntry.RotationZ, rotationZ);
    }

    [TestMethod]
    public void TestTryReadWithInvalidKind()
    {
        var observer = new StubObserver();

        var element = JsonSerializer.SerializeToElement(null as object);

        var reader = new MetaAtchEntryReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = reader.TryRead(element, out var metaAtchEntry);

        Assert.IsFalse(success);
        Assert.IsNull(metaAtchEntry);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual("Expected [Object] value kind but found [Null]: ", actualMessage));
    }

    [TestMethod]
    public void TestTryReadWithInvalidEmptyBone()
    {
        var observer = new StubObserver();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>
        {
            { nameof(MetaAtchEntry.Bone), string.Empty }
        });

        var reader = new MetaAtchEntryReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = reader.TryRead(element, out var metaAtchEntry);

        Assert.IsFalse(success);
        Assert.IsNull(metaAtchEntry);

        var calls = observer.GetCalls();
        Assert.HasCount(2, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Debug),
            actualMessage => Assert.AreEqual("Expected value to not be empty", actualMessage));
        AssertPluginLog.MatchObservedCall(calls[1], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected property [Bone] value to not be empty [String]: {element}", actualMessage));
    }

    [TestMethod]
    [DataRow(nameof(MetaAtchEntry.Scale), JsonValueKind.Number)]
    [DataRow(nameof(MetaAtchEntry.OffsetX), JsonValueKind.Number)]
    [DataRow(nameof(MetaAtchEntry.OffsetY), JsonValueKind.Number)]
    [DataRow(nameof(MetaAtchEntry.OffsetZ), JsonValueKind.Number)]
    [DataRow(nameof(MetaAtchEntry.RotationX), JsonValueKind.Number)]
    [DataRow(nameof(MetaAtchEntry.RotationY), JsonValueKind.Number)]
    [DataRow(nameof(MetaAtchEntry.RotationZ), JsonValueKind.Number)]
    public void TestTryReadWithInvalidValueKind(string propertyName, JsonValueKind kind)
    {
        var observer = new StubObserver();

        var value = new Dictionary<string, object?>
        {
            { nameof(MetaAtchEntry.Bone), "Bone" },
            { nameof(MetaAtchEntry.Scale), 1 },
            { nameof(MetaAtchEntry.OffsetX), 2 },
            { nameof(MetaAtchEntry.OffsetY), 3 },
            { nameof(MetaAtchEntry.OffsetZ), 4 },
            { nameof(MetaAtchEntry.RotationX), 5 },
            { nameof(MetaAtchEntry.RotationY), 6 },
            { nameof(MetaAtchEntry.RotationZ), 7 },
        };

        var propertyValue = false;

        value[propertyName] = propertyValue;

        var element = JsonSerializer.SerializeToElement(value);

        var reader = new MetaAtchEntryReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = reader.TryRead(element, out var metaAtchEntry);

        Assert.IsFalse(success);
        Assert.IsNull(metaAtchEntry);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [Number] value kind but found [False]: {propertyValue}", actualMessage));
    }
}
