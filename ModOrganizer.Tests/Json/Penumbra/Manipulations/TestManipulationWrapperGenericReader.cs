
using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Manipulations;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Json.Readers;
using ModOrganizer.Tests.Json.Readers.Asserts;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.Penumbra.Manipulations;

[TestClass]
public class TestManipulationWrapperGenericReader
{
    [TestMethod]
    public void TestTryReadWithInvalidType()
    {
        var observer = new StubObserver();

        var type = "Unknown";

        var manipulationWrapperGenericReader = new ManipulationWrapperGenericReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithAssertIsValue(true)
            .WithAssertIsValuePresent(type)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { "Type", type }
        });

        var success = manipulationWrapperGenericReader.TryRead(element, out var manipulationWrapper);

        Assert.IsFalse(success);
        Assert.IsNull(manipulationWrapper);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Failed to get [ManipulationWrapper] reader for type [{type}] (registered types: Atch, Atr, Eqdp, Eqp, Est, GlobalEqp, Gmp, Imc, Rsp, Shp): {element}", actualMessage));
    }


    [TestMethod]
    [DataRow("Atch")]
    [DataRow("Atr")]
    [DataRow("Eqdp")]
    [DataRow("Eqp")]
    [DataRow("Est")]
    [DataRow("GlobalEqp")]
    [DataRow("Imc")]
    [DataRow("Rsp")]
    [DataRow("Shp")]
    public void TestTryReadWithType(string type)
    {
        var manipulationWrapper = new ManipulationWrapper()
        {
            Type = type,
            Manipulation = null!
        };

        var manipulationWrapperGenericReader = new ManipulationWrapperGenericReaderBuilder()
            .WithAssertIsValue(true)
            .WithAssertIsValuePresent(type)
            .WithReaderTryRead(type, manipulationWrapper)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            {"Type", type}
        });

        var success = manipulationWrapperGenericReader.TryRead(element, out var actualManipulationWrapper);

        Assert.IsTrue(success);
        Assert.AreSame(manipulationWrapper, actualManipulationWrapper);
    }

    [TestMethod]
    [DataRow("Atch")]
    [DataRow("Atr")]
    [DataRow("Eqdp")]
    [DataRow("Eqp")]
    [DataRow("Est")]
    [DataRow("GlobalEqp")]
    [DataRow("Imc")]
    [DataRow("Rsp")]
    [DataRow("Shp")]
    public void TestTryReadWithTypeWithoutSuccess(string type)
    {
        var observer = new StubObserver();

        var manipulationWrapperGenericReader = new ManipulationWrapperGenericReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithAssertIsValue(true)
            .WithAssertIsValuePresent(type)
            .WithReaderTryRead(type, null as ManipulationWrapper)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { "Type", type }
        });

        var success = manipulationWrapperGenericReader.TryRead(element, out var manipulationWrapper);

        Assert.IsFalse(success);
        Assert.IsNull(manipulationWrapper);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Debug),
            actualMessage => Assert.StartsWith($"Failed to read [ManipulationWrapper] using reader", actualMessage));
    }
}
