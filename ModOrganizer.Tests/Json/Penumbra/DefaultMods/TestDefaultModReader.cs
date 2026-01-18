using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Containers;
using ModOrganizer.Json.Penumbra.DefaultMods;
using ModOrganizer.Json.Penumbra.Manipulations;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Json.Penumbra.Containers;
using System;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.Penumbra.DefaultMods;

[TestClass]
public class TestDefaultModReader
{
    [TestMethod]
    public void TestTryRead()
    {
        var filesOrFileSwaps = new Dictionary<string, string>();
        var manipulations = Array.Empty<ManipulationWrapper>();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(DefaultMod.Files), filesOrFileSwaps },
            { nameof(DefaultMod.FileSwaps), filesOrFileSwaps },
            { nameof(DefaultMod.Manipulations), manipulations }
        });

        var container = new Container()
        {
            Files = filesOrFileSwaps,
            FileSwaps = filesOrFileSwaps,
            Manipulations = manipulations
        };

        var defaultModReader = new DefaultModReaderBuilder()
            .WithContainerReaderTryRead(container)
            .Build();

        var success = defaultModReader.TryRead(element, out var defaultMod);

        Assert.IsTrue(success);
        Assert.IsNotNull(defaultMod);

        Assert.AreSame(filesOrFileSwaps, defaultMod.Files);
        Assert.AreSame(filesOrFileSwaps, defaultMod.FileSwaps);
        Assert.AreSame(manipulations, defaultMod.Manipulations);
    }

    [TestMethod]
    public void TestTryReadWithInvalidKind()
    {
        var observer = new StubObserver();

        var element = JsonSerializer.SerializeToElement(null as object);

        var defaultModReader = new DefaultModReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = defaultModReader.TryRead(element, out var defaultMod);

        Assert.IsFalse(success);
        Assert.IsNull(defaultMod);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual("Expected value kind [Object] but found [Null]: ", actualMessage));
    }

    [TestMethod]
    public void TestTryReadWithInvalidVersion()
    {
        var observer = new StubObserver();

        var version = 1;
        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(DefaultMod.Version), version }
        });

        var defaultModReader = new DefaultModReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = defaultModReader.TryRead(element, out var defaultMod);

        Assert.IsFalse(success);
        Assert.IsNull(defaultMod);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), 
            actualMessage => Assert.AreEqual($"Failed to read [DefaultMod], unsupported [Version] found [{version}] (supported version: 0): {element}", actualMessage));
    }

    [TestMethod]
    public void TestTryReadWithInvalidBaseContainer()
    {
        var observer = new StubObserver();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>());

        var defaultModReader = new DefaultModReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithContainerReaderTryRead(null)
            .Build();

        var success = defaultModReader.TryRead(element, out var defaultMod);

        Assert.IsFalse(success);
        Assert.IsNull(defaultMod);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Debug),
            actualMessage => Assert.AreEqual($"Failed to read base [Container] for [DefaultMod]: {element}", actualMessage));
    }
}
