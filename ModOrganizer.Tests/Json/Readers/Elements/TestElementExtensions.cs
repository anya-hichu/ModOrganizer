using Dalamud.Plugin.Services;
using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Readers.Elements;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using System.Numerics;
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
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var element = JsonSerializer.Deserialize<JsonElement>(json);

        var success = element.Is(element.ValueKind, pluginLogStub);

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
    public void TestIsWithoutSuccess(string json)
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

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, null } });

        var success = element.HasProperty(propertyName);

        Assert.IsTrue(success);
    }

    [TestMethod]
    public void TestHasPropertyWithoutSuccess()
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>());

        var success = element.HasProperty("Property Name", pluginLogStub);

        Assert.IsFalse(success);
    }

    [TestMethod]
    public void TestHasPropertyWithNullElement()
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var element = JsonSerializer.SerializeToElement(null as object);

        var success = element.HasProperty("Property Name", pluginLogStub);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [Object] value kind but found [{element.ValueKind}]: {element}", actualMessage));
    }

    [TestMethod]
    public void TestTryGetOptionalProperty()
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, true } });

        var success = element.TryGetOptionalProperty(propertyName, out var property, pluginLogStub);

        Assert.IsTrue(success);
        Assert.AreEqual(JsonValueKind.True, property.ValueKind);
    }

    [TestMethod]
    public void TestTryGetOptionalPropertyWithNullValue()
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, null } });

        var success = element.TryGetOptionalProperty(propertyName, out var property, pluginLogStub);

        Assert.IsFalse(success);
        Assert.AreEqual(default, property);
    }

    [TestMethod]
    public void TestTryGetOptionalPropertyWithNullElement()
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var element = JsonSerializer.SerializeToElement(null as object);

        var success = element.TryGetOptionalProperty("Property Name", out var property, pluginLogStub);

        Assert.IsFalse(success);
        Assert.AreEqual(default, property);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [Object] value kind but found [{element.ValueKind}]: {element}", actualMessage));
    }

    [TestMethod]
    public void TestTryGetOptionalStringPropertyValue()
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var propertyName = "Property Name";
        var propertyValue = "Property Value";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetOptionalPropertyValue(propertyName, out string? actualValue, pluginLogStub);

        Assert.IsTrue(success);
        Assert.AreEqual(propertyValue, actualValue);
    }

    [TestMethod]
    public void TestTryGetOptionalStringPropertyValueWithNull()
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, null } });

        var success = element.TryGetOptionalPropertyValue(propertyName, out string? value, pluginLogStub);

        Assert.IsTrue(success);
        Assert.IsNull(value);
    }

    [TestMethod]
    public void TestTryGetOptionalStringPropertyValueWithInvalidKind()
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var propertyName = "Property Name";
        var propertyValue = 0;

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetOptionalPropertyValue(propertyName, out string? value, pluginLogStub);

        Assert.IsFalse(success);
        Assert.IsNull(value);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [String] value kind but found [Number]: {propertyValue}", actualMessage));
    }

    [TestMethod]
    public void TestTryGetOptionalBooleanPropertyValue()
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var propertyName = "Property Name";
        var propertyValue = true;

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetOptionalPropertyValue(propertyName, out bool? actualValue, pluginLogStub);

        Assert.IsTrue(success);
        Assert.AreEqual(propertyValue, actualValue);
    }

    [TestMethod]
    public void TestTryGetOptionalBooleanPropertyValueWithNull()
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, null } });

        var success = element.TryGetOptionalPropertyValue(propertyName, out bool? value, pluginLogStub);

        Assert.IsTrue(success);
        Assert.IsNull(value);
    }

    [TestMethod]
    public void TestTryGetOptionalBooleanPropertyValueWithInvalidKind()
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var propertyName = "Property Name";
        var propertyValue = 0;

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetOptionalPropertyValue(propertyName, out bool? value, pluginLogStub);

        Assert.IsFalse(success);
        Assert.IsNull(value);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [Number] value kind to be parsable as [Boolean]: {propertyValue}", actualMessage));
    }

    [TestMethod]
    [DataRow(byte.MinValue)]
    [DataRow(byte.MaxValue)]
    public void TestTryGetOptionalBytePropertyValue(byte propertyValue)
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetOptionalPropertyValue(propertyName, out ushort? actualValue, pluginLogStub);

        Assert.IsTrue(success);
        Assert.AreEqual(propertyValue, actualValue);
    }

    [TestMethod]
    public void TestTryGetOptionalUShortPropertyValueWithNull()
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, null } });

        var success = element.TryGetOptionalPropertyValue(propertyName, out ushort? value, pluginLogStub);

        Assert.IsTrue(success);
        Assert.IsNull(value);
    }

    [TestMethod]
    [DataRow(ushort.MinValue - 1)]
    [DataRow(ushort.MaxValue + 1)]
    public void TestTryGetOptionalUShortPropertyValueWithInvalidValue(int propertyValue)
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetOptionalPropertyValue(propertyName, out ushort? value, pluginLogStub);

        Assert.IsFalse(success);
        Assert.IsNull(value);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [Number] value kind to be parsable as [UInt16]: {propertyValue}", actualMessage));
    }

    [TestMethod]
    public void TestTryGetOptionalUShortPropertyValueWithInvalidKind()
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var propertyName = "Property Name";
        var propertyValue = false;

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetOptionalPropertyValue(propertyName, out ushort? value, pluginLogStub);

        Assert.IsFalse(success);
        Assert.IsNull(value);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [Number] value kind but found [False]: {propertyValue}", actualMessage));
    }

    [TestMethod]
    [DataRow(uint.MinValue)]
    [DataRow(uint.MaxValue)]
    public void TestTryGetOptionalUIntPropertyValue(uint propertyValue)
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetOptionalPropertyValue(propertyName, out uint? actualValue, pluginLogStub);

        Assert.IsTrue(success);
        Assert.AreEqual(propertyValue, actualValue);
    }

    [TestMethod]
    public void TestTryGetOptionalUIntPropertyValueWithNull()
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, null } });

        var success = element.TryGetOptionalPropertyValue(propertyName, out uint? value, pluginLogStub);

        Assert.IsTrue(success);
        Assert.IsNull(value);
    }

    [TestMethod]
    [DataRow(uint.MinValue - 1L)]
    [DataRow(uint.MaxValue + 1L)]
    public void TestTryGetOptionalUIntPropertyValueWithInvalidValue(long propertyValue)
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetOptionalPropertyValue(propertyName, out uint? value, pluginLogStub);

        Assert.IsFalse(success);
        Assert.IsNull(value);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [Number] value kind to be parsable as [UInt32]: {propertyValue}", actualMessage));
    }

    [TestMethod]
    public void TestTryGetOptionalUIntPropertyValueWithInvalidKind()
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var propertyName = "Property Name";
        var propertyValue = false;

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetOptionalPropertyValue(propertyName, out uint? value, pluginLogStub);

        Assert.IsFalse(success);
        Assert.IsNull(value);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [Number] value kind but found [False]: {propertyValue}", actualMessage));
    }

    [TestMethod]
    [DataRow(int.MinValue)]
    [DataRow(int.MaxValue)]
    public void TestTryGetOptionalIntPropertyValue(int propertyValue)
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetOptionalPropertyValue(propertyName, out int? actualValue, pluginLogStub);

        Assert.IsTrue(success);
        Assert.AreEqual(propertyValue, actualValue);
    }

    [TestMethod]
    public void TestTryGetOptionalIntPropertyValueWithNull()
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, null } });

        var success = element.TryGetOptionalPropertyValue(propertyName, out int? value, pluginLogStub);

        Assert.IsTrue(success);
        Assert.IsNull(value);
    }

    [TestMethod]
    [DataRow(int.MinValue - 1L)]
    [DataRow(int.MaxValue + 1L)]
    public void TestTryGetOptionalIntPropertyValueWithInvalidValue(long propertyValue)
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetOptionalPropertyValue(propertyName, out int? value, pluginLogStub);

        Assert.IsFalse(success);
        Assert.IsNull(value);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [Number] value kind to be parsable as [Int32]: {propertyValue}", actualMessage));
    }

    [TestMethod]
    public void TestTryGetOptionalIntPropertyValueWithInvalidKind()
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var propertyName = "Property Name";
        var propertyValue = false;

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetOptionalPropertyValue(propertyName, out int? value, pluginLogStub);

        Assert.IsFalse(success);
        Assert.IsNull(value);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [Number] value kind but found [False]: {propertyValue}", actualMessage));
    }

    [TestMethod]
    [DataRow(int.MinValue)]
    [DataRow(int.MaxValue)]
    public void TestTryGetOptionalLongPropertyValue(int propertyValue)
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetOptionalPropertyValue(propertyName, out long? actualValue, pluginLogStub);

        Assert.IsTrue(success);
        Assert.AreEqual(propertyValue, actualValue);
    }

    [TestMethod]
    public void TestTryGetOptionalLongPropertyValueWithNull()
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, null } });

        var success = element.TryGetOptionalPropertyValue(propertyName, out long? value, pluginLogStub);

        Assert.IsTrue(success);
        Assert.IsNull(value);
    }

    [TestMethod]
    [DataRow(long.MinValue, -1)]
    [DataRow(long.MaxValue, 1)]
    public void TestTryGetOptionalLongPropertyValueWithInvalidValue(long limit, int offset)
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var propertyName = "Property Name";
        var propertyValue = new BigInteger(limit) + offset;

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, (Int128)propertyValue } });

        var success = element.TryGetOptionalPropertyValue(propertyName, out long? value, pluginLogStub);

        Assert.IsFalse(success);
        Assert.IsNull(value);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [Number] value kind to be parsable as [Int64]: {propertyValue}", actualMessage));
    }

    [TestMethod]
    public void TestTryGetOptionalLongPropertyValueWithInvalidKind()
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var propertyName = "Property Name";
        var propertyValue = false;

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetOptionalPropertyValue(propertyName, out long? value, pluginLogStub);

        Assert.IsFalse(success);
        Assert.IsNull(value);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [Number] value kind but found [False]: {propertyValue}", actualMessage));
    }
}
