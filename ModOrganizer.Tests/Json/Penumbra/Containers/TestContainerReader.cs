using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Containers;
using ModOrganizer.Json.Readers;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Json.Penumbra.Manipulations;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.Penumbra.Containers;

[TestClass]
public class TestContainerReader
{
    [TestMethod]
    public void TestTryRead()
    {
        var observer = new StubObserver();

        var fileKey = "File Key";
        var fileValue = "File Value";

        var fileSwapKey = "File Swap Key";
        var fileSwapValue = "File Swap Value";

        var value = new Dictionary<string, object?>()
        {
            { nameof(Container.Files), new Dictionary<string, object?>() { { fileKey, fileValue } } },
            { nameof(Container.FileSwaps), new Dictionary<string, object?>() { { fileSwapKey, fileSwapValue } } },
            { nameof(Container.Manipulations), Array.Empty<object?>() }
        };

        var jsonElement = JsonSerializer.SerializeToElement(value);

        var containerReader = new ContainerReaderBuilder()
            .WithManipulationWrapperReaderObserver(observer)
            .WithManipulationWrapperReaderTryReadMany([])
            .Build();

        var success = containerReader.TryRead(jsonElement, out var container);

        Assert.IsTrue(success);
        Assert.IsNotNull(container);

        Assert.IsNotNull(container.Files);
        Assert.HasCount(1, container.Files);
        Assert.AreEqual(fileValue, container.Files[fileKey]);

        Assert.IsNotNull(container.FileSwaps);
        Assert.HasCount(1, container.FileSwaps);
        Assert.AreEqual(fileSwapValue, container.FileSwaps[fileSwapKey]);

        Assert.IsNotNull(container.Manipulations);
        Assert.IsEmpty(container.Manipulations);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        var call = calls[0];
        Assert.AreEqual(nameof(IReader<>.TryReadMany), call.StubbedMethod.Name);

        switch (call.GetArguments()[0])
        {
            case JsonElement manipulationsProperty:
                Assert.AreEqual(JsonValueKind.Array, manipulationsProperty.ValueKind);
                Assert.AreEqual(0, manipulationsProperty.GetArrayLength());
                break;
            default:
                Assert.Fail("Expected first call argument to be a JsonElement");
                break;
        }
    }

    [TestMethod]
    public void TestTryReadWithDefaults()
    {
        var jsonElement = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() 
        {
            { nameof(Container.Files), null },
            { nameof(Container.FileSwaps), null },
            { nameof(Container.Manipulations), null }
        });

        var success = new ContainerReaderBuilder()
            .Build()
            .TryRead(jsonElement, out var container);

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

        var jsonElement = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(Container.Files), string.Empty }
        });

        var containerReader = new ContainerReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = containerReader.TryRead(jsonElement, out var container);

        Assert.IsFalse(success);
        Assert.IsNull(container);

        var calls = observer.GetCalls();

        Assert.HasCount(2, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), actualMessage => Assert.AreEqual("Expected value kind [Object] but got [String]: ", actualMessage));
        AssertPluginLog.MatchObservedCall(calls[1], nameof(IPluginLog.Warning), actualMessage => Assert.AreEqual("Failed to read one or more [Files] for [Container]: ", actualMessage));
    }

    [TestMethod]
    public void TestTryReadWithInvalidFileSwaps()
    {
        var observer = new StubObserver();

        var jsonElement = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() 
        {
            { nameof(Container.FileSwaps), string.Empty }
        });

        var containerReader = new ContainerReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = containerReader.TryRead(jsonElement, out var container);

        Assert.IsFalse(success);
        Assert.IsNull(container);

        var calls = observer.GetCalls();

        Assert.HasCount(2, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), actualMessage => Assert.AreEqual("Expected value kind [Object] but got [String]: ", actualMessage));
        AssertPluginLog.MatchObservedCall(calls[1], nameof(IPluginLog.Warning), actualMessage => Assert.AreEqual("Failed to read one or more [FileSwaps] for [Container]: ", actualMessage));
    }

    [TestMethod]
    public void TestTryReadWithInvalidManipulations()
    {
        var observer = new StubObserver();

        var jsonElement = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() 
        {
            { nameof(Container.Manipulations), string.Empty }
        });

        var containerReader = new ContainerReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithManipulationWrapperReaderTryReadMany(null)
            .Build();

        var success = containerReader.TryRead(jsonElement, out var container);

        Assert.IsFalse(success);
        Assert.IsNull(container);

        var calls = observer.GetCalls();

        Assert.HasCount(1, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), actualMessage => Assert.AreEqual("Failed to read one or more [Manipulations] for [Container]: ", actualMessage));
    }
}
