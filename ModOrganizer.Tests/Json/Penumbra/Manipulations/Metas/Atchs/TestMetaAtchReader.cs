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
    private static readonly MetaAtchEntry DEFAULT_ENTRY = new()
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

    [TestMethod]
    public void TestTryRead()
    {
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
            .WithMetaAtchEntryReaderTryRead(DEFAULT_ENTRY)
            .Build();

        var success = reader.TryRead(element, out var metaAtch);

        Assert.IsTrue(success);
        Assert.IsNotNull(metaAtch);

        Assert.AreSame(DEFAULT_ENTRY, metaAtch.Entry);
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

    [TestMethod]
    public void TestTryReadWithInvalidEntry()
    {
        var observer = new StubObserver();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(MetaAtch.Entry), new Dictionary<string, object?>() }
        });

        var reader = new MetaAtchReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithMetaAtchEntryReaderTryRead(null)
            .Build();

        var success = reader.TryRead(element, out var metaAtch);

        Assert.IsFalse(success);
        Assert.IsNull(metaAtch);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Debug),
            actualMessage => Assert.AreEqual($"Failed to read [MetaAtchEntry] for [MetaAtch]: {element}", actualMessage));
    }

    [TestMethod]
    public void TestTryReadWithInvalidType()
    {
        var observer = new StubObserver();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(MetaAtch.Entry), new Dictionary<string, object?>() },
            { nameof(MetaAtch.Gender), "Gender" },
            { nameof(MetaAtch.Race), "Race" },
            { nameof(MetaAtch.Type), string.Empty },
            { nameof(MetaAtch.Index), 1 }
        });

        var reader = new MetaAtchReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithMetaAtchEntryReaderTryRead(DEFAULT_ENTRY)
            .Build();

        var success = reader.TryRead(element, out var metaAtch);

        Assert.IsFalse(success);
        Assert.IsNull(metaAtch);

        var calls = observer.GetCalls();
        Assert.HasCount(2, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Debug),
            actualMessage => Assert.AreEqual("Expected value to not be empty", actualMessage));
        AssertPluginLog.MatchObservedCall(calls[1], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected property [Type] value to not be empty [String]: {element}", actualMessage));
    }

    [TestMethod]
    [DataRow(nameof(MetaAtch.Gender))]
    [DataRow(nameof(MetaAtch.Race))]
    [DataRow(nameof(MetaAtch.Index))]
    public void TestTryReadWithMissingProperty(string propertyName)
    {
        var observer = new StubObserver();

        var value = new Dictionary<string, object?>()
        {
            { nameof(MetaAtch.Entry), new Dictionary<string, object?>() },
            { nameof(MetaAtch.Gender), "Gender" },
            { nameof(MetaAtch.Race), "Race" },
            { nameof(MetaAtch.Type), "Type" },
            { nameof(MetaAtch.Index), 1 }
        };

        value.Remove(propertyName);

        var element = JsonSerializer.SerializeToElement(value);

        var reader = new MetaAtchReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithMetaAtchEntryReaderTryRead(DEFAULT_ENTRY)
            .Build();

        var success = reader.TryRead(element, out var metaAtch);

        Assert.IsFalse(success);
        Assert.IsNull(metaAtch);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected property [{propertyName}] to be present: {element}", actualMessage));
    }
}
