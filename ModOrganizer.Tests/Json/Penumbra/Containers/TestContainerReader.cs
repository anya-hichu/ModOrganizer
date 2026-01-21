using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Containers;
using ModOrganizer.Json.Penumbra.Manipulations;
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
        var filePath = "FilePath";
        var fileNewPath = "File New Path";

        var fileSwapPath = "File Swap Path";
        var fileSwapNewPath = "File Swap New Path";

        var manipulations = Array.Empty<ManipulationWrapper>();

        var value = new Dictionary<string, object?>()
        {
            { nameof(Container.Files), new Dictionary<string, string>() {
                { filePath, fileNewPath }
            }},
            { nameof(Container.FileSwaps), new Dictionary<string, string>() { 
                { fileSwapPath, fileSwapNewPath }
            }},
            { nameof(Container.Manipulations), manipulations }
        };

        var element = JsonSerializer.SerializeToElement(value);

        var containerReader = new ContainerReaderBuilder()
            .WithManipulationWrapperGenericReaderTryReadMany(manipulations)
            .Build();

        var success = containerReader.TryRead(element, out var container);

        Assert.IsTrue(success);
        Assert.IsNotNull(container);

        Assert.IsNotNull(container.Files);
        Assert.HasCount(1, container.Files);
        Assert.AreEqual(fileNewPath, container.Files[filePath]);

        Assert.IsNotNull(container.FileSwaps);
        Assert.HasCount(1, container.FileSwaps);
        Assert.AreEqual(fileSwapNewPath, container.FileSwaps[fileSwapPath]);

        Assert.AreSame(manipulations, container.Manipulations);
    }

    [TestMethod]
    public void TestTryReadWithoutProperties()
    {
        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>());

        var containerReader = new ContainerReaderBuilder().Build();

        var success = containerReader.TryRead(element, out var container);

        Assert.IsTrue(success);
        Assert.IsNotNull(container);

        Assert.IsNull(container.Files);
        Assert.IsNull(container.FileSwaps);
        Assert.IsNull(container.Manipulations);
    }

    [TestMethod]
    public void TestTryReadWithNullPropertyValues()
    {
        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() 
        {
            { nameof(Container.Files), null },
            { nameof(Container.FileSwaps), null },
            { nameof(Container.Manipulations), null }
        });

        var containerReader = new ContainerReaderBuilder().Build();

        var success = containerReader.TryRead(element, out var container);

        Assert.IsTrue(success);
        Assert.IsNotNull(container);

        Assert.IsNull(container.Files);
        Assert.IsNull(container.FileSwaps);
        Assert.IsNull(container.Manipulations);
    }

    [TestMethod]
    public void TestTryReadWithInvalidFiles()
    {
        var observer = new StubObserver();

        var files = "Invalid Files";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(Container.Files), files }
        });

        var containerReader = new ContainerReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = containerReader.TryRead(element, out var container);

        Assert.IsFalse(success);
        Assert.IsNull(container);

        var calls = observer.GetCalls();

        Assert.HasCount(1, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected value kind [Object] but found [String]: {files}", actualMessage));
    }

    [TestMethod]
    public void TestTryReadWithInvalidFileSwaps()
    {
        var observer = new StubObserver();

        var fileSwaps = "Invalid File Swaps";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(Container.FileSwaps), fileSwaps }
        });

        var containerReader = new ContainerReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = containerReader.TryRead(element, out var container);

        Assert.IsFalse(success);
        Assert.IsNull(container);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected value kind [Object] but found [String]: {fileSwaps}", actualMessage));
    }

    [TestMethod]
    public void TestTryReadWithInvalidManipulations()
    {
        var observer = new StubObserver();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() 
        {
            { nameof(Container.Manipulations), "Invalid Manipulations" }
        });

        var containerReader = new ContainerReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithManipulationWrapperGenericReaderTryReadMany(null)
            .Build();

        var success = containerReader.TryRead(element, out var container);

        Assert.IsFalse(success);
        Assert.IsNull(container);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), 
            actualMessage => Assert.AreEqual($"Failed to read one or more [Manipulations] for [Container]: {element}", actualMessage));
    }
}
