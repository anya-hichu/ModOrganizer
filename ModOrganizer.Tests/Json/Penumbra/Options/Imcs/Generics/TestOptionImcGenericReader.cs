using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Options.Imcs.AttributeMasks;
using ModOrganizer.Json.Penumbra.Options.Imcs.IsDisableSubMods;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Json.Penumbra.Options.Imcs.AttributeMasks;
using ModOrganizer.Tests.Json.Penumbra.Options.Imcs.IsDisableSubMods;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.Penumbra.Options.Imcs.Generics;

[TestClass]
public class TestOptionImcGenericReader
{
    [TestMethod]
    public void TestTryReadAttributeMask()
    {
        var optionImc = new OptionImcAttributeMask()
        {
            Name = string.Empty,
            AttributeMask = default
        };

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(OptionImcAttributeMask.AttributeMask), null }
        });

        var reader = new OptionImcGenericReaderBuilder()
            .WithOptionImcAttributeMaskReaderTryRead(optionImc)
            .WithOptionImcIsDisableSubModReaderTryRead(null)
            .Build();

        var success = reader.TryRead(element, out var actualOptionImc);

        Assert.IsTrue(success);
        Assert.AreSame(optionImc, actualOptionImc);
    }

    [TestMethod]
    public void TestTryReadIsDisabledSubMod()
    {
        var optionImc = new OptionImcIsDisableSubMod()
        {
            Name = string.Empty,
            IsDisableSubMod = false
        };

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(OptionImcIsDisableSubMod.IsDisableSubMod), null }
        });

        var reader = new OptionImcGenericReaderBuilder()
            .WithOptionImcAttributeMaskReaderTryRead(null)
            .WithOptionImcIsDisableSubModReaderTryRead(optionImc)
            .Build();

        var success = reader.TryRead(element, out var actualOptionImc);

        Assert.IsTrue(success);
        Assert.AreSame(optionImc, actualOptionImc);
    }

    [TestMethod]
    public void TestTryReadWithInvalidKind()
    {
        var observer = new StubObserver();

        var element = JsonSerializer.SerializeToElement(null as object);

        var reader = new OptionImcGenericReaderBuilder()
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
    public void TestTryReadWithoutAttributes()
    {
        var observer = new StubObserver();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>());

        var reader = new OptionImcGenericReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = reader.TryRead(element, out var optionImc);

        Assert.IsFalse(success);
        Assert.IsNull(optionImc);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual("Failed to determine reader for [OptionImc], both properties [AttributeMask] and [IsDisableSubMod] are missing", actualMessage));
    }

    [TestMethod]
    public void TestTryReadWithBothAttributes()
    {
        var observer = new StubObserver();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(OptionImcAttributeMask.AttributeMask), null },
            { nameof(OptionImcIsDisableSubMod.IsDisableSubMod), null }
        });

        var reader = new OptionImcGenericReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = reader.TryRead(element, out var optionImc);

        Assert.IsFalse(success);
        Assert.IsNull(optionImc);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual("Failed to determine reader for [OptionImc], both properties [AttributeMask] and [IsDisableSubMod] are present", actualMessage));
    }
}
