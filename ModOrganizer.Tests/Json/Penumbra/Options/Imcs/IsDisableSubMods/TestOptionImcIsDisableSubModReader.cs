using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Options;
using ModOrganizer.Json.Penumbra.Options.Imcs.IsDisableSubMods;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.Penumbra.Options.Imcs.IsDisableSubMods;

[TestClass]
public class TestOptionImcIsDisableSubModReader
{
    [TestMethod]
    public void TestTryRead()
    {
        var name = "Option Name";

        var option = new Option()
        {
            Name = name
        };

        var isDisableSubMod = true;

        var reader = new OptionImcIsDisableSubModReaderBuilder()
            .WithOptionReaderTryRead(option)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(OptionImcIsDisableSubMod.IsDisableSubMod), isDisableSubMod }
        });

        var success = reader.TryRead(element, out var optionImc);

        Assert.IsTrue(success);
        Assert.IsNotNull(optionImc);

        Assert.AreEqual(name, optionImc.Name);

        var optionImcIsDisableSubMod = optionImc as OptionImcIsDisableSubMod;

        Assert.IsNotNull(optionImcIsDisableSubMod);
        Assert.AreEqual(isDisableSubMod, optionImcIsDisableSubMod.IsDisableSubMod);
    }

    [TestMethod]
    public void TestTryReadWithInvalidKind()
    {
        var observer = new StubObserver();

        var element = JsonSerializer.SerializeToElement(null as object);

        var reader = new OptionImcIsDisableSubModReaderBuilder()
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

        var reader = new OptionImcIsDisableSubModReaderBuilder()
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
            actualMessage => Assert.AreEqual($"Failed to read base [Option] for [OptionImcIsDisableSubMod]: {element}", actualMessage));
    }

    [TestMethod]
    public void TestTryReadWithInvalidValueKind()
    {
        var observer = new StubObserver();

        var option = new Option()
        {
            Name = string.Empty
        };

        var isDisableSubMod = 0;

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(OptionImcIsDisableSubMod.IsDisableSubMod), isDisableSubMod }
        });

        var reader = new OptionImcIsDisableSubModReaderBuilder()
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
            actualMessage => Assert.AreEqual($"Expected [Number] value kind to be parsable as [Boolean]: {isDisableSubMod}", actualMessage));
    }
}
