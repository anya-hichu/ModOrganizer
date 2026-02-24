using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Atchs;
using ModOrganizer.Json.Penumbra.Manipulations.Wrappers;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.Penumbra.Manipulations.Metas.Atchs.Wrappers;

[TestClass]
public class TestMetaAtchWrapperReader
{
    [TestMethod]
    public void TestTryRead()
    {
        var type = MetaAtchWrapperReader.TYPE;

        var metaAtch = new MetaAtch()
        {
            Entry = null!,
            Gender = string.Empty,
            Race = string.Empty,
            Type = type,
            Index = default,
        };

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(ManipulationWrapper.Manipulation), new Dictionary<string, object?>() }
        });

        var reader = new MetaAtchWrapperReaderBuilder()
            .WithMetaAtchReaderTryRead(metaAtch)
            .Build();

        var success = reader.TryRead(element, out var metaAtchWrapper);

        Assert.IsTrue(success);
        Assert.IsNotNull(metaAtchWrapper);

        Assert.AreEqual(type, metaAtchWrapper.Type);
        Assert.AreSame(metaAtch, metaAtchWrapper.Manipulation);
    }

    [TestMethod]
    public void TestTryReadWithInvalidKind()
    {
        var observer = new StubObserver();

        var element = JsonSerializer.SerializeToElement(null as object);

        var reader = new MetaAtchWrapperReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = reader.TryRead(element, out var metaAtchWrapper);

        Assert.IsFalse(success);
        Assert.IsNull(metaAtchWrapper);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual("Expected [Object] value kind but found [Null]: ", actualMessage));
    }

    [TestMethod]
    public void TestTryReadWithoutManipulation()
    {
        var observer = new StubObserver();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>());

        var reader = new MetaAtchWrapperReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = reader.TryRead(element, out var metaAtchWrapper);

        Assert.IsFalse(success);
        Assert.IsNull(metaAtchWrapper);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected property [Manipulation] to be present: {element}", actualMessage));
    }

    [TestMethod]
    public void TestTryReadWithInvalidMeta()
    {
        var observer = new StubObserver();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(ManipulationWrapper.Manipulation), new Dictionary<string, object?>() }
        });

        var reader = new MetaAtchWrapperReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithMetaAtchReaderTryRead(null)
            .Build();

        var success = reader.TryRead(element, out var metaAtchWrapper);

        Assert.IsFalse(success);
        Assert.IsNull(metaAtchWrapper);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Debug),
            actualMessage => Assert.AreEqual($"Failed to read wrapped [MetaAtch] for [ManipulationWrapper]: {element}", actualMessage));
    }
}
