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

        var success = reader.TryRead(element, out var entry);

        Assert.IsTrue(success);
        Assert.IsNotNull(entry);

        Assert.AreEqual(entry.Bone, bone);
        Assert.AreEqual(entry.Scale, scale);
        Assert.AreEqual(entry.OffsetX, offsetX);
        Assert.AreEqual(entry.OffsetY, offsetY);
        Assert.AreEqual(entry.OffsetZ, offsetZ);
        Assert.AreEqual(entry.RotationX, rotationX);
        Assert.AreEqual(entry.RotationY, rotationY);
        Assert.AreEqual(entry.RotationZ, rotationZ);
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

        var success = reader.TryRead(element, out var entry);

        Assert.IsFalse(success);
        Assert.IsNull(entry);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual("Expected [Object] value kind but found [Null]: ", actualMessage));
    }
}
