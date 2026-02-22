using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Containers;
using ModOrganizer.Json.Penumbra.Manipulations.Wrappers;
using ModOrganizer.Json.Penumbra.Options;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Json.Penumbra.Containers;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.Penumbra.Options.Containers;

[TestClass]
public class TestOptionContainerReader
{
    [TestMethod]
    public void TestTryRead()
    {
        var files = new Dictionary<string, string>();
        var fileSwaps = new Dictionary<string, string>();
        var manipulations = Array.Empty<ManipulationWrapper>();

        var container = new Container()
        {
            Files = files,
            FileSwaps = fileSwaps,
            Manipulations = manipulations
        };

        var name = "Option Name";

        var option = new Option()
        {
            Name = name
        };

        var reader = new OptionContainerReaderBuilder()
            .WithContainerReaderTryRead(container)
            .WithOptionReaderTryRead(option)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>());

        var success = reader.TryRead(element, out var optionContainer);

        Assert.IsTrue(success);
        Assert.IsNotNull(optionContainer);

        Assert.AreEqual(name, optionContainer.Name);

        Assert.AreSame(files, optionContainer.Files);
        Assert.AreSame(fileSwaps, optionContainer.FileSwaps);
        Assert.AreSame(manipulations, optionContainer.Manipulations);
    }

    [TestMethod]
    public void TestTryReadWithInvalidKind()
    {
        var observer = new StubObserver();

        var reader = new OptionContainerReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var element = JsonSerializer.SerializeToElement(null as object);

        var success = reader.TryRead(element, out var optionContainer);

        Assert.IsFalse(success);
        Assert.IsNull(optionContainer);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual("Expected [Object] value kind but found [Null]: ", actualMessage));
    }
}
