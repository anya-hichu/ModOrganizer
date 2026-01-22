using Dalamud.Plugin.Services;
using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Readers.Elements;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.Readers.Elements;

[TestClass]
public class TestElementExtensions
{
    [TestMethod]
    [DataRow("0")]
    [DataRow("[]")]
    [DataRow("{}")]
    [DataRow("null")]
    [DataRow("true")]
    [DataRow("false")]
    [DataRow(""" "" """)]
    public void TestIs(string json)
    {
        var element = JsonSerializer.Deserialize<JsonElement>(json);

        var success = element.Is(element.ValueKind);

        Assert.IsTrue(success);
    }

    [TestMethod]
    [DataRow("0")]
    [DataRow("[]")]
    [DataRow("{}")]
    [DataRow("null")]
    [DataRow("true")]
    [DataRow("false")]
    [DataRow(""" "" """)]
    public void TestIsWithInvalidTypes(string json)
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var element = JsonSerializer.Deserialize<JsonElement>(json);

        var success = element.Is(JsonValueKind.Undefined, pluginLogStub);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [Undefined] value kind but found [{element.ValueKind}]: {element}", actualMessage));
    }

    [TestMethod]
    public void TestHasProperty()
    {
        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { propertyName, null }
        });

        var success = element.HasProperty(propertyName);

        Assert.IsTrue(success);
    }

    [TestMethod]
    public void TestHasPropertyWithoutSuccess()
    {
        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>());

        var success = element.HasProperty("Property Name");

        Assert.IsFalse(success);
    }

    /*
    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("Value")]
    public void TestTryGetStringValue(string value)
    {
        var element = new ElementBuilder()
            .WithValue(value)
            .Build();

        var success = element.TryGetValue(out string? actualValue);

        Assert.IsTrue(success);
        Assert.AreEqual(value, actualValue);
    }

    [TestMethod]
    [DataRow("0")]
    [DataRow("{}")]
    [DataRow("[]")]
    [DataRow("null")]
    [DataRow("true")]
    [DataRow("false")]
    public void TestTryGetStringValueWithInvalidTypes(string jsonValue)
    {
        var observer = new StubObserver();

        var builder = new ElementBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithJsonValue(jsonValue);

        var element = builder.Build();

        var success = element.TryGetValue(out string? actualValue, builder.PluginLogStub);

        Assert.IsFalse(success);
        Assert.IsNull(actualValue);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [String] value kind but found [{element.ValueKind}]: {element}", actualMessage));
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void TestTryGetBoolValue(bool value)
    {
        var element = new ElementBuilder()
            .WithValue(value)
            .Build();

        var success = element.TryGetValue(out bool actualValue);

        Assert.IsTrue(success);
        Assert.AreEqual(value, actualValue);
    }

    [TestMethod]
    [DataRow("0")]
    [DataRow("{}")]
    [DataRow("[]")]
    [DataRow("null")]
    [DataRow(""" "" """)]
    public void TestTryGetBoolValueWithInvalidTypes(string jsonValue)
    {
        var observer = new StubObserver();

        var builder = new ElementBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithJsonValue(jsonValue);

        var element = builder.Build();

        var success = element.TryGetValue(out bool actualValue, builder.PluginLogStub);

        Assert.IsFalse(success);
        Assert.AreEqual(default, actualValue);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected value kind [{element.ValueKind}] to be parsable as [Boolean]: {element}", actualMessage));
    }

    [TestMethod]
    [DataRow(byte.MinValue)]
    [DataRow(byte.MaxValue)]
    public void TestTryGetByteValue(byte value)
    {
        var element = new ElementBuilder()
            .WithValue(value)
            .Build();

        var success = element.TryGetValue(out byte actualValue);

        Assert.IsTrue(success);
        Assert.AreEqual(value, actualValue);
    }

    [TestMethod]
    [DataRow("{}")]
    [DataRow("[]")]
    [DataRow("null")]
    [DataRow("true")]
    [DataRow("false")]
    [DataRow(""" "" """)]
    public void TestTryGetByteValueWithInvalidTypes(string jsonValue)
    {
        var observer = new StubObserver();

        var builder = new ElementBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithJsonValue(jsonValue);

        var element = builder.Build();

        var success = element.TryGetValue(out byte actualValue, builder.PluginLogStub);

        Assert.IsFalse(success);
        Assert.AreEqual(default, actualValue);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [Number] value kind but found [{element.ValueKind}]: {element}", actualMessage));
    }

    [TestMethod]
    [DataRow(ushort.MinValue - 1)]
    [DataRow(ushort.MaxValue + 1)]
    public void TestTryGetByteValueWithInvalidValues(int value)
    {
        var observer = new StubObserver();

        var builder = new ElementBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithValue(value);

        var element = builder.Build();

        var success = element.TryGetValue(out byte actualValue, builder.PluginLogStub);

        Assert.IsFalse(success);
        Assert.AreEqual(default, actualValue);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected value kind [{element.ValueKind}] to be parsable as [Byte]: {element}", actualMessage));
    }
    */
}
