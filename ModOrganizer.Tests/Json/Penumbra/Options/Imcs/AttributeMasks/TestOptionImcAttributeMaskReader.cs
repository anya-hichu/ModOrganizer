using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Options;
using ModOrganizer.Json.Penumbra.Options.Imcs.AttributeMasks;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.Penumbra.Options.Imcs.AttributeMasks;

[TestClass]
public class TestOptionImcAttributeMaskReader
{
    [TestMethod]
    public void TestTryRead()
    {
        var name = "Option Name";

        var option = new Option()
        {
            Name = name
        };

        var attributeMask = 0;

        var reader = new OptionImcAttributeMaskReaderBuilder()
            .WithOptionReaderTryRead(option)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(OptionImcAttributeMask.AttributeMask), attributeMask }
        });

        var success = reader.TryRead(element, out var optionImc);

        Assert.IsTrue(success);
        Assert.IsNotNull(optionImc);

        Assert.AreEqual(name, optionImc.Name);

        var optionImcAttributeMask = optionImc as OptionImcAttributeMask;

        Assert.IsNotNull(optionImcAttributeMask);
        Assert.AreEqual(attributeMask, optionImcAttributeMask.AttributeMask);
    }

    [TestMethod]
    public void TestTryReadWithInvalidKind()
    {
        var observer = new StubObserver();

        var element = JsonSerializer.SerializeToElement(null as object);

        var reader = new OptionImcAttributeMaskReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = reader.TryRead(element, out var optionImc);

        Assert.IsFalse(success);
        Assert.IsNull(optionImc);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual("Expected [Object] value kind but found [Null]: ", actualMessage));
    }

    [TestMethod]
    public void TestTryReadWithInvalidOption()
    {
        var observer = new StubObserver();

        var reader = new OptionImcAttributeMaskReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithOptionReaderTryRead(null)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>());

        var success = reader.TryRead(element, out var optionImc);

        Assert.IsFalse(success);
        Assert.IsNull(optionImc);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Debug),
            actualMessage => Assert.AreEqual($"Failed to read base [Option] for [OptionImcAttributeMask]: {element}", actualMessage));
    }

    [TestMethod]
    public void TestTryReadWithInvalidValueKind()
    {
        var observer = new StubObserver();

        var option = new Option()
        {
            Name = string.Empty
        };

        var attributeMask = false;

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(OptionImcAttributeMask.AttributeMask), attributeMask }
        });

        var reader = new OptionImcAttributeMaskReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithOptionReaderTryRead(option)
            .Build();

        var success = reader.TryRead(element, out var optionImc);

        Assert.IsFalse(success);
        Assert.IsNull(optionImc);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [Number] value kind but found [False]: {attributeMask}", actualMessage));
    }
}
