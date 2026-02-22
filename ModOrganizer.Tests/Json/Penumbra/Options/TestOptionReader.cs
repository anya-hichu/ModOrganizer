using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Options;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.Penumbra.Options;

[TestClass]
public class TestOptionReader
{
    [TestMethod]
    public void TestTryRead()
    {
        var name = "Option Name";
        var description = "Option Description";
        var priority = 0;
        var image = "Option image";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(Option.Name), name },
            { nameof(Option.Description), description },
            { nameof(Option.Priority), priority },
            { nameof(Option.Image), image }
        });

        var reader = new OptionReaderBuilder().Build();

        var success = reader.TryRead(element, out var option);

        Assert.IsTrue(success);
        Assert.IsNotNull(option);

        Assert.AreEqual(name, option.Name);
        Assert.AreEqual(description, option.Description);
        Assert.AreEqual(priority, option.Priority);
        Assert.AreEqual(image, option.Image);
    }

    [TestMethod]
    public void TestTryReadWithInvalidKind()
    {
        var observer = new StubObserver();

        var element = JsonSerializer.SerializeToElement(null as object);

        var reader = new OptionReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = reader.TryRead(element, out var option);

        Assert.IsFalse(success);
        Assert.IsNull(option);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual("Expected [Object] value kind but found [Null]: ", actualMessage));
    }

    [TestMethod]
    public void TryTryReadWithInvalidName()
    {
        var observer = new StubObserver();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>());

        var reader = new OptionReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = reader.TryRead(element, out var option);

        Assert.IsFalse(success);
        Assert.IsNull(option);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected property [Name] to be present: {element}", actualMessage));
    }

    [TestMethod]
    [DataRow(nameof(Option.Description), JsonValueKind.String)]
    [DataRow(nameof(Option.Priority), JsonValueKind.Number)]
    [DataRow(nameof(Option.Image), JsonValueKind.String)]
    public void TryTryReadWithInvalidValueKind(string propertyName, JsonValueKind kind)
    {
        var observer = new StubObserver();

        var propertyValue = false;

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(Option.Name), string.Empty },
            { propertyName, propertyValue }
        });

        var reader = new OptionReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = reader.TryRead(element, out var option);

        Assert.IsFalse(success);
        Assert.IsNull(option);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [{kind}] value kind but found [False]: {propertyValue}", actualMessage));
    }
}
