using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Atchs;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Atchs.Entries;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Json.Penumbra.Manipulations.Metas.Atchs.Entries;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.Penumbra.Manipulations.Metas.Atchs;

[TestClass]
public class TestMetaAtchReader
{
    [TestMethod]
    public void TestTryRead()
    {
        var entry = new MetaAtchEntry()
        {
            Bone = string.Empty,
            Scale = default,
            OffsetX = default,
            OffsetY = default,
            OffsetZ = default,
            RotationX = default,
            RotationY = default,
            RotationZ = default
        };

        var gender = "Gender";
        var race = "Race";
        var type = "Type";
        var index = 0;

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(MetaAtch.Entry), new Dictionary<string, object?>() },
            { nameof(MetaAtch.Gender), gender },
            { nameof(MetaAtch.Race), race },
            { nameof(MetaAtch.Type), type },
            { nameof(MetaAtch.Index), index }
        });

        var reader = new MetaAtchReaderBuilder()
            .WithMetaAtchEntryReaderTryRead(entry)
            .Build();

        var success = reader.TryRead(element, out var metaAtch);

        Assert.IsTrue(success);
        Assert.IsNotNull(metaAtch);

        Assert.AreSame(entry, metaAtch.Entry);
        Assert.AreEqual(gender, metaAtch.Gender);
        Assert.AreEqual(race, metaAtch.Race);
        Assert.AreEqual(type, metaAtch.Type);
        Assert.AreEqual(index, metaAtch.Index);
    }

    [TestMethod]
    public void TestTryReadWithInvalidKind()
    {
        var observer = new StubObserver();

        var element = JsonSerializer.SerializeToElement(null as object);

        var reader = new MetaAtchReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = reader.TryRead(element, out var metaAtch);

        Assert.IsFalse(success);
        Assert.IsNull(metaAtch);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual("Expected [Object] value kind but found [Null]: ", actualMessage));
    }
}
