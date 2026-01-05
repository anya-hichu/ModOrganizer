using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Containers;
using ModOrganizer.Json.Penumbra.Manipulations;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Json.Penumbra.Manipulations;
using ModOrganizer.Tests.Json.Readers.Asserts;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.Penumbra.Containers;

[TestClass]
public class TestContainerReader
{
    [TestMethod]
    public void TestTryRead()
    {
        var filesOrFileSwaps = new Dictionary<string, string>();
        var manipulations = Array.Empty<ManipulationWrapper>();

        var value = new Dictionary<string, object?>()
        {
            { nameof(Container.Files), filesOrFileSwaps },
            { nameof(Container.FileSwaps), filesOrFileSwaps },
            { nameof(Container.Manipulations), manipulations }
        };

        var element = JsonSerializer.SerializeToElement(value);

        var containerReader = new ContainerReaderBuilder()
            .WithAssertIsValue(true)
            .WithAssertIsStringDict(filesOrFileSwaps)
            .WithManipulationWrapperReaderTryReadMany(manipulations)
            .Build();

        var success = containerReader.TryRead(element, out var container);

        Assert.IsTrue(success);
        Assert.IsNotNull(container);

        Assert.AreSame(filesOrFileSwaps, container.Files);
        Assert.AreSame(filesOrFileSwaps, container.FileSwaps);

        Assert.AreSame(manipulations, container.Manipulations);
    }

    [TestMethod]
    public void TestTryReadWithDefaults()
    {
        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() 
        {
            { nameof(Container.Files), null },
            { nameof(Container.FileSwaps), null },
            { nameof(Container.Manipulations), null }
        });

        var containerReader = new ContainerReaderBuilder()
            .WithAssertIsValue(true)
            .Build();

        var success = containerReader.TryRead(element, out var container);

        Assert.IsTrue(success);
        Assert.IsNotNull(container);

        Assert.IsNotNull(container.Files);
        Assert.IsEmpty(container.Files);

        Assert.IsNotNull(container.FileSwaps);
        Assert.IsEmpty(container.FileSwaps);

        Assert.IsNotNull(container.Manipulations);
        Assert.IsEmpty(container.Manipulations);
    }

    [TestMethod]
    public void TestTryReadWithInvalidFiles()
    {
        var observer = new StubObserver();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(Container.Files), false },
            { nameof(Container.FileSwaps), true },
            { nameof(Container.Manipulations), true }
        });

        var containerReader = new ContainerReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithAssertIsValue(true)
            .WithAssertIsStringDictSuccessfulOnTrue()
            .WithManipulationWrapperReaderTryReadManySuccessfulOnTrue()
            .Build();

        var success = containerReader.TryRead(element, out var container);

        Assert.IsFalse(success);
        Assert.IsNull(container);

        var calls = observer.GetCalls();

        Assert.HasCount(1, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), actualMessage => Assert.AreEqual($"Failed to read one or more [Files] for [Container]: {element}", actualMessage));
    }

    [TestMethod]
    public void TestTryReadWithInvalidFileSwaps()
    {
        var observer = new StubObserver();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(Container.Files), true },
            { nameof(Container.FileSwaps), false },
            { nameof(Container.Manipulations), true }
        });

        var containerReader = new ContainerReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithAssertIsValue(true)
            .WithAssertIsStringDictSuccessfulOnTrue()
            .WithManipulationWrapperReaderTryReadManySuccessfulOnTrue()
            .Build();

        var success = containerReader.TryRead(element, out var container);

        Assert.IsFalse(success);
        Assert.IsNull(container);

        var calls = observer.GetCalls();

        Assert.HasCount(1, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), actualMessage => Assert.AreEqual($"Failed to read one or more [FileSwaps] for [Container]: {element}", actualMessage));
    }

    [TestMethod]
    public void TestTryReadWithInvalidManipulations()
    {
        var observer = new StubObserver();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() 
        {
            { nameof(Container.Files), true },
            { nameof(Container.FileSwaps), true },
            { nameof(Container.Manipulations), false }
        });

        var containerReader = new ContainerReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithAssertIsValue(true)
            .WithAssertIsStringDictSuccessfulOnTrue()
            .WithManipulationWrapperReaderTryReadManySuccessfulOnTrue()
            .Build();

        var success = containerReader.TryRead(element, out var container);

        Assert.IsFalse(success);
        Assert.IsNull(container);

        var calls = observer.GetCalls();

        Assert.HasCount(1, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), actualMessage => Assert.AreEqual($"Failed to read one or more [Manipulations] for [Container]: {element}", actualMessage));
    }
}
