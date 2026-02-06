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

        var success = element.TryGetOptionalProperty(propertyName, out var _, pluginLogStub);

        Assert.IsFalse(success);
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

        var success = element.TryGetOptionalProperty("Property Name", out var _, pluginLogStub);

        Assert.IsFalse(success);

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
    [DataRow(ushort.MinValue)]
    [DataRow(ushort.MaxValue)]
    public void TestTryGetOptionalUShortPropertyValue(ushort propertyValue)
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

    [TestMethod]
    public void TestTryGetOptionalStringDictPropertyValue()
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var dictKey = "Dict Key";
        var dictValue = "Dict Value";

        var propertyName = "Property Name";
        var propertyValue = new Dictionary<string, object?>() { { dictKey, dictValue } };

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetOptionalPropertyValue(propertyName, out Dictionary<string, string>? actualValue, pluginLogStub);

        Assert.IsTrue(success);
        Assert.IsNotNull(actualValue);

        Assert.HasCount(1, actualValue);
        Assert.AreEqual(dictValue, actualValue[dictKey]);
    }

    [TestMethod]
    public void TestTryGetOptionalStringDictPropertyValueWithNull()
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, null } });

        var success = element.TryGetOptionalPropertyValue(propertyName, out Dictionary<string, string>? value, pluginLogStub);

        Assert.IsTrue(success);
        Assert.IsNull(value);
    }

    [TestMethod]
    public void TestTryGetOptionalStringDictPropertyValueWithInvalidKind()
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var dictKey = "Dict Key";
        var dictValue = 0;

        var propertyName = "Property Name";
        var propertyValue = new Dictionary<string, object?>() { { dictKey, dictValue } };

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetOptionalPropertyValue(propertyName, out Dictionary<string, string>? value, pluginLogStub);

        Assert.IsFalse(success);
        Assert.IsNull(value);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [String] value kind but found [Number]: {dictValue}", actualMessage));
    }

    [TestMethod]
    public void TestTryGetOptionalStringArrayPropertyValue()
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var arrayValue = "Array Value";

        var propertyName = "Property Name";
        var propertyValue = new object?[] { arrayValue };

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetOptionalPropertyValue(propertyName, out string[]? actualValue, pluginLogStub);

        Assert.IsTrue(success);
        Assert.IsNotNull(actualValue);

        Assert.HasCount(1, actualValue);
        Assert.AreEqual(arrayValue, actualValue[0]);
    }

    [TestMethod]
    public void TestTryGetOptionalStringArrayPropertyValueWithNull()
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, null } });

        var success = element.TryGetOptionalPropertyValue(propertyName, out string[]? value, pluginLogStub);

        Assert.IsTrue(success);
        Assert.IsNull(value);
    }

    [TestMethod]
    public void TestTryGetOptionalStringArrayPropertyValueWithInvalidKind()
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

        var success = element.TryGetOptionalPropertyValue(propertyName, out string[]? value, pluginLogStub);

        Assert.IsFalse(success);
        Assert.IsNull(value);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [Array] value kind but found [Number]: {propertyValue}", actualMessage));
    }

    [TestMethod]
    public void TestTryGetOptionalStringArrayPropertyValueWithInvalidItemKind()
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var itemValue = 0;

        var propertyName = "Property Name";
        var propertyValue = new object?[] { itemValue };

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetOptionalPropertyValue(propertyName, out string[]? value, pluginLogStub);

        Assert.IsFalse(success);
        Assert.IsNull(value);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [String] value kind but found [Number]: {itemValue}", actualMessage));
    }

    [TestMethod]
    public void TestTryGetOptionalIntArrayPropertyValue()
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var arrayValue = 0;

        var propertyName = "Property Name";
        var propertyValue = new object?[] { arrayValue };

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetOptionalPropertyValue(propertyName, out int[]? actualValue, pluginLogStub);

        Assert.IsTrue(success);
        Assert.IsNotNull(actualValue);

        Assert.HasCount(1, actualValue);
        Assert.AreEqual(arrayValue, actualValue[0]);
    }

    [TestMethod]
    public void TestTryGetOptionalIntArrayPropertyValueWithNull()
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, null } });

        var success = element.TryGetOptionalPropertyValue(propertyName, out int[]? value, pluginLogStub);

        Assert.IsTrue(success);
        Assert.IsNull(value);
    }

    [TestMethod]
    public void TestTryGetOptionalIntArrayPropertyValueWithInvalidKind()
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

        var success = element.TryGetOptionalPropertyValue(propertyName, out int[]? value, pluginLogStub);

        Assert.IsFalse(success);
        Assert.IsNull(value);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [Array] value kind but found [Number]: {propertyValue}", actualMessage));
    }

    [TestMethod]
    public void TestTryGetOptionalIntArrayPropertyValueWithInvalidItemKind()
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var itemValue = "Item Value";

        var propertyName = "Property Name";
        var propertyValue = new object?[] { itemValue };

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetOptionalPropertyValue(propertyName, out int[]? value, pluginLogStub);

        Assert.IsFalse(success);
        Assert.IsNull(value);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [Number] value kind but found [String]: {itemValue}", actualMessage));
    }

    [TestMethod]
    [DataRow(0, ushort.MinValue)]
    [DataRow("0", ushort.MinValue)]
    [DataRow(65535, ushort.MaxValue)]
    [DataRow("65535", ushort.MaxValue)]
    public void TestTryGetOptionalU16PropertyValue(object? propertyValue, ushort expectedValue)
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetOptionalU16PropertyValue(propertyName, out var actualValue, pluginLogStub);

        Assert.IsTrue(success);
        Assert.AreEqual(expectedValue, actualValue);
    }

    [TestMethod]
    public void TestTryGetOptionalU16PropertyValueWithNull()
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, null } });

        var success = element.TryGetOptionalU16PropertyValue(propertyName, out var value, pluginLogStub);

        Assert.IsTrue(success);
        Assert.IsNull(value);
    }

    [TestMethod]
    [DataRow(ushort.MinValue - 1, JsonValueKind.Number)]
    [DataRow("-1", JsonValueKind.String)]
    [DataRow(ushort.MaxValue + 1, JsonValueKind.Number)]
    [DataRow("65536", JsonValueKind.String)]
    public void TestTryGetOptionalU16PropertyValueWithInvalidValue(object? propertyValue, JsonValueKind propertyValueKind)
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetOptionalU16PropertyValue(propertyName, out var value, pluginLogStub);

        Assert.IsFalse(success);
        Assert.IsNull(value);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [{propertyValueKind}] value kind to be parsable as [UInt16]: {propertyValue}", actualMessage));
    }

    [TestMethod]
    public void TestTryGetOptionalU16PropertyValueWithInvalidKind()
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

        var success = element.TryGetOptionalU16PropertyValue(propertyName, out var value, pluginLogStub);

        Assert.IsFalse(success);
        Assert.IsNull(value);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [False] value kind to be parsable as [UInt16]: {propertyValue}", actualMessage));
    }

    [TestMethod]
    public void TestTryGetRequiredProperty()
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, true } });

        var success = element.TryGetRequiredProperty(propertyName, out var property, pluginLogStub);

        Assert.IsTrue(success);
        Assert.AreEqual(JsonValueKind.True, property.ValueKind);
    }

    [TestMethod]
    public void TestTryGetRequiredPropertyWithNullValue()
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, null } });

        var success = element.TryGetRequiredProperty(propertyName, out var _, pluginLogStub);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected property [{propertyName}] to be present: {element}", actualMessage));
    }

    [TestMethod]
    public void TestTryGetRequiredPropertyWithNullElement()
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var element = JsonSerializer.SerializeToElement(null as object);

        var success = element.TryGetRequiredProperty("Property Name", out var _, pluginLogStub);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [Object] value kind but found [{element.ValueKind}]: {element}", actualMessage));
    }


    [TestMethod]
    public void TestTryGetRequiredStringPropertyValue()
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var propertyName = "Property Name";
        var propertyValue = "Property Value";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetRequiredPropertyValue(propertyName, out string? actualValue, pluginLogStub);

        Assert.IsTrue(success);
        Assert.AreEqual(propertyValue, actualValue);
    }

    [TestMethod]
    public void TestTryGetRequiredStringPropertyValueWithNull()
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, null } });

        var success = element.TryGetRequiredPropertyValue(propertyName, out string? value, pluginLogStub);

        Assert.IsFalse(success);
        Assert.IsNull(value);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected property [{propertyName}] to be present: {element}", actualMessage));
    }

    [TestMethod]
    public void TestTryGetRequiredStringPropertyValueWithInvalidKind()
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

        var success = element.TryGetRequiredPropertyValue(propertyName, out string? value, pluginLogStub);

        Assert.IsFalse(success);
        Assert.IsNull(value);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [String] value kind but found [Number]: {propertyValue}", actualMessage));
    }


    [TestMethod]
    public void TestTryGetRequiredBooleanPropertyValue()
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var propertyName = "Property Name";
        var propertyValue = true;

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetRequiredPropertyValue(propertyName, out bool actualValue, pluginLogStub);

        Assert.IsTrue(success);
        Assert.AreEqual(propertyValue, actualValue);
    }

    [TestMethod]
    public void TestTryGetRequiredBooleanPropertyValueWithNull()
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, null } });

        var success = element.TryGetRequiredPropertyValue(propertyName, out bool _, pluginLogStub);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected property [{propertyName}] to be present: {element}", actualMessage));
    }

    [TestMethod]
    public void TestTryGetRequiredBooleanPropertyValueWithInvalidKind()
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

        var success = element.TryGetRequiredPropertyValue(propertyName, out bool _, pluginLogStub);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [Number] value kind to be parsable as [Boolean]: {propertyValue}", actualMessage));
    }

    [TestMethod]
    [DataRow(byte.MinValue)]
    [DataRow(byte.MaxValue)]
    public void TestTryGetRequiredBytePropertyValue(byte propertyValue)
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetRequiredPropertyValue(propertyName, out byte actualValue, pluginLogStub);

        Assert.IsTrue(success);
        Assert.AreEqual(propertyValue, actualValue);
    }

    [TestMethod]
    public void TestTryGetRequiredBytePropertyValueWithNull()
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, null } });

        var success = element.TryGetRequiredPropertyValue(propertyName, out byte _, pluginLogStub);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected property [{propertyName}] to be present: {element}", actualMessage));
    }

    [TestMethod]
    [DataRow(byte.MinValue - 1)]
    [DataRow(byte.MaxValue + 1)]
    public void TestTryGetRequiredBytePropertyValueWithInvalidValue(int propertyValue)
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetRequiredPropertyValue(propertyName, out byte _, pluginLogStub);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [Number] value kind to be parsable as [Byte]: {propertyValue}", actualMessage));
    }

    [TestMethod]
    public void TestTryGetRequiredBytePropertyValueWithInvalidKind()
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

        var success = element.TryGetRequiredPropertyValue(propertyName, out byte _, pluginLogStub);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [Number] value kind but found [False]: {propertyValue}", actualMessage));
    }

    [TestMethod]
    [DataRow(ushort.MinValue)]
    [DataRow(ushort.MaxValue)]
    public void TestTryGetRequiredUShortPropertyValue(ushort propertyValue)
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetRequiredPropertyValue(propertyName, out ushort actualValue, pluginLogStub);

        Assert.IsTrue(success);
        Assert.AreEqual(propertyValue, actualValue);
    }

    [TestMethod]
    public void TestTryGetRequiredUShortPropertyValueWithNull()
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, null } });

        var success = element.TryGetRequiredPropertyValue(propertyName, out ushort _, pluginLogStub);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected property [{propertyName}] to be present: {element}", actualMessage));
    }

    [TestMethod]
    [DataRow(ushort.MinValue - 1)]
    [DataRow(ushort.MaxValue + 1)]
    public void TestTryGetRequiredUShortPropertyValueWithInvalidValue(int propertyValue)
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetRequiredPropertyValue(propertyName, out ushort value, pluginLogStub);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [Number] value kind to be parsable as [UInt16]: {propertyValue}", actualMessage));
    }

    [TestMethod]
    public void TestTryGetRequiredUShortPropertyValueWithInvalidKind()
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

        var success = element.TryGetRequiredPropertyValue(propertyName, out ushort _, pluginLogStub);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [Number] value kind but found [False]: {propertyValue}", actualMessage));
    }

    [TestMethod]
    [DataRow(uint.MinValue)]
    [DataRow(uint.MaxValue)]
    public void TestTryGetRequiredUIntPropertyValue(uint propertyValue)
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetRequiredPropertyValue(propertyName, out uint actualValue, pluginLogStub);

        Assert.IsTrue(success);
        Assert.AreEqual(propertyValue, actualValue);
    }

    [TestMethod]
    public void TestTryGetRequiredUIntPropertyValueWithNull()
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, null } });

        var success = element.TryGetRequiredPropertyValue(propertyName, out uint _, pluginLogStub);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected property [{propertyName}] to be present: {element}", actualMessage));
    }

    [TestMethod]
    [DataRow(uint.MinValue - 1L)]
    [DataRow(uint.MaxValue + 1L)]
    public void TestTryGetRequiredUIntPropertyValueWithInvalidValue(long propertyValue)
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetRequiredPropertyValue(propertyName, out uint _, pluginLogStub);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [Number] value kind to be parsable as [UInt32]: {propertyValue}", actualMessage));
    }

    [TestMethod]
    public void TestTryGetRequiredUIntPropertyValueWithInvalidKind()
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

        var success = element.TryGetRequiredPropertyValue(propertyName, out uint _, pluginLogStub);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [Number] value kind but found [False]: {propertyValue}", actualMessage));
    }

    [TestMethod]
    [DataRow(ulong.MinValue)]
    [DataRow(ulong.MaxValue)]
    public void TestTryGetRequiredULongPropertyValue(ulong propertyValue)
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetRequiredPropertyValue(propertyName, out ulong actualValue, pluginLogStub);

        Assert.IsTrue(success);
        Assert.AreEqual(propertyValue, actualValue);
    }

    [TestMethod]
    public void TestTryGetRequiredULongPropertyValueWithNull()
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, null } });

        var success = element.TryGetRequiredPropertyValue(propertyName, out ulong _, pluginLogStub);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected property [{propertyName}] to be present: {element}", actualMessage));
    }

    [TestMethod]
    [DataRow(ulong.MinValue, -1)]
    [DataRow(ulong.MaxValue, 1)]
    public void TestTryGetRequiredULongPropertyValueWithInvalidValue(ulong limit, int offset)
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

        var success = element.TryGetRequiredPropertyValue(propertyName, out ulong _, pluginLogStub);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [Number] value kind to be parsable as [UInt64]: {propertyValue}", actualMessage));
    }

    [TestMethod]
    public void TestTryGetRequiredULongPropertyValueWithInvalidKind()
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

        var success = element.TryGetRequiredPropertyValue(propertyName, out ulong _, pluginLogStub);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [Number] value kind but found [False]: {propertyValue}", actualMessage));
    }

    [TestMethod]
    [DataRow(int.MinValue)]
    [DataRow(int.MaxValue)]
    public void TestTryGetRequiredIntPropertyValue(int propertyValue)
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetRequiredPropertyValue(propertyName, out int actualValue, pluginLogStub);

        Assert.IsTrue(success);
        Assert.AreEqual(propertyValue, actualValue);
    }

    [TestMethod]
    public void TestTryGetRequiredIntPropertyValueWithNull()
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, null } });

        var success = element.TryGetRequiredPropertyValue(propertyName, out int _, pluginLogStub);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected property [{propertyName}] to be present: {element}", actualMessage));
    }

    [TestMethod]
    [DataRow(int.MinValue - 1L)]
    [DataRow(int.MaxValue + 1L)]
    public void TestTryGetRequiredIntPropertyValueWithInvalidValue(long propertyValue)
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetRequiredPropertyValue(propertyName, out int _, pluginLogStub);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [Number] value kind to be parsable as [Int32]: {propertyValue}", actualMessage));
    }

    [TestMethod]
    public void TestTryGetRequiredIntPropertyValueWithInvalidKind()
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

        var success = element.TryGetRequiredPropertyValue(propertyName, out int _, pluginLogStub);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [Number] value kind but found [False]: {propertyValue}", actualMessage));
    }

    [TestMethod]
    [DataRow(float.MinValue)]
    [DataRow(float.MaxValue)]
    public void TestTryGetRequiredFloatPropertyValue(float propertyValue)
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetRequiredPropertyValue(propertyName, out float actualValue, pluginLogStub);

        Assert.IsTrue(success);
        Assert.AreEqual(propertyValue, actualValue);
    }

    [TestMethod]
    public void TestTryGetRequiredFloatPropertyValueWithNull()
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, null } });

        var success = element.TryGetRequiredPropertyValue(propertyName, out float value, pluginLogStub);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected property [{propertyName}] to be present: {element}", actualMessage));
    }

    [TestMethod]
    [DataRow(double.MinValue, float.NegativeInfinity)]
    [DataRow(double.MaxValue, float.PositiveInfinity)]
    public void TestTryGetRequiredFloatPropertyValueWithInvalidValue(double propertyValue, float expectedValue)
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetRequiredPropertyValue(propertyName, out float value, pluginLogStub);

        Assert.IsTrue(success);
        Assert.AreEqual(expectedValue, value);
    }

    [TestMethod]
    public void TestTryGetRequiredFloatPropertyValueWithInvalidKind()
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

        var success = element.TryGetRequiredPropertyValue(propertyName, out float value, pluginLogStub);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [Number] value kind but found [False]: {propertyValue}", actualMessage));
    }


    [TestMethod]
    public void TestTryGetRequiredStringDictPropertyValue()
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var dictKey = "Dict Key";
        var dictValue = "Dict Value";

        var propertyName = "Property Name";
        var propertyValue = new Dictionary<string, object?>() { { dictKey, dictValue } };

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetRequiredPropertyValue(propertyName, out Dictionary<string, string>? actualValue, pluginLogStub);

        Assert.IsTrue(success);
        Assert.IsNotNull(actualValue);

        Assert.HasCount(1, actualValue);
        Assert.AreEqual(dictValue, actualValue[dictKey]);
    }

    [TestMethod]
    public void TestTryGetRequiredStringDictPropertyValueWithNull()
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, null } });

        var success = element.TryGetRequiredPropertyValue(propertyName, out Dictionary<string, string>? value, pluginLogStub);

        Assert.IsFalse(success);
        Assert.IsNull(value);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected property [{propertyName}] to be present: {element}", actualMessage));
    }

    [TestMethod]
    public void TestTryGetRequiredStringDictPropertyValueWithInvalidKind()
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var dictKey = "Dict Key";
        var dictValue = 0;

        var propertyName = "Property Name";
        var propertyValue = new Dictionary<string, object?>() { { dictKey, dictValue } };

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetRequiredPropertyValue(propertyName, out Dictionary<string, string>? value, pluginLogStub);

        Assert.IsFalse(success);
        Assert.IsNull(value);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [String] value kind but found [Number]: {dictValue}", actualMessage));
    }


    [TestMethod]
    public void TestTryGetRequiredStringArrayPropertyValue()
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var arrayValue = "Array Value";

        var propertyName = "Property Name";
        var propertyValue = new object?[] { arrayValue };

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetRequiredPropertyValue(propertyName, out string[]? actualValue, pluginLogStub);

        Assert.IsTrue(success);
        Assert.IsNotNull(actualValue);

        Assert.HasCount(1, actualValue);
        Assert.AreEqual(arrayValue, actualValue[0]);
    }

    [TestMethod]
    public void TestTryGetRequiredStringArrayPropertyValueWithNull()
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, null } });

        var success = element.TryGetRequiredPropertyValue(propertyName, out string[]? value, pluginLogStub);

        Assert.IsFalse(success);
        Assert.IsNull(value);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected property [{propertyName}] to be present: {element}", actualMessage));
    }

    [TestMethod]
    public void TestTryGetRequiredStringArrayPropertyValueWithInvalidKind()
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

        var success = element.TryGetRequiredPropertyValue(propertyName, out string[]? value, pluginLogStub);

        Assert.IsFalse(success);
        Assert.IsNull(value);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [Array] value kind but found [Number]: {propertyValue}", actualMessage));
    }

    [TestMethod]
    public void TestTryGetRequiredStringArrayPropertyValueWithInvalidItemKind()
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var itemValue = 0;

        var propertyName = "Property Name";
        var propertyValue = new object?[] { itemValue };

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetRequiredPropertyValue(propertyName, out string[]? value, pluginLogStub);

        Assert.IsFalse(success);
        Assert.IsNull(value);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [String] value kind but found [Number]: {itemValue}", actualMessage));
    }

    [TestMethod]
    public void TestTryGetRequiredNotEmptyStringPropertyValue()
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var propertyName = "Property Name";
        var propertyValue = "Property Value";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetRequiredNotEmptyPropertyValue(propertyName, out var actualValue, pluginLogStub);

        Assert.IsTrue(success);
        Assert.AreEqual(propertyValue, actualValue);
    }

    [TestMethod]
    public void TestTryGetRequiredNotEmptyStringPropertyValueWithoutSuccess()
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var propertyName = "Property Name";
        var propertyValue = string.Empty;

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetRequiredNotEmptyPropertyValue(propertyName, out var actualValue, pluginLogStub);

        Assert.IsFalse(success);
        Assert.IsNull(actualValue);

        var calls = observer.GetCalls();
        Assert.HasCount(2, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Debug),
            actualMessage => Assert.AreEqual("Expected value to not be empty", actualMessage));

        AssertPluginLog.MatchObservedCall(calls[1], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected property [{propertyName}] value to be not empty [String]: {element}", actualMessage));
    }

    [TestMethod]
    public void TestTryGetRequiredNotEmptyStringPropertyValueWithNull()
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, null } });

        var success = element.TryGetRequiredNotEmptyPropertyValue(propertyName, out var value, pluginLogStub);

        Assert.IsFalse(success);
        Assert.IsNull(value);

        var calls = observer.GetCalls();
        Assert.HasCount(2, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected property [{propertyName}] to be present: {element}", actualMessage));

        AssertPluginLog.MatchObservedCall(calls[1], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected property [{propertyName}] value to be not empty [String]: {element}", actualMessage));
    }

    [TestMethod]
    public void TestTryGetRequiredNotEmptyStringPropertyValueWithInvalidKind()
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

        var success = element.TryGetRequiredNotEmptyPropertyValue(propertyName, out var value, pluginLogStub);

        Assert.IsFalse(success);
        Assert.IsNull(value);

        var calls = observer.GetCalls();
        Assert.HasCount(2, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [String] value kind but found [Number]: {propertyValue}", actualMessage));

        AssertPluginLog.MatchObservedCall(calls[1], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected property [{propertyName}] value to be not empty [String]: {element}", actualMessage));
    }

    [TestMethod]
    [DataRow(0, byte.MinValue)]
    [DataRow("0", byte.MinValue)]
    [DataRow(255, byte.MaxValue)]
    [DataRow("255", byte.MaxValue)]
    public void TestTryGetRequiredU8PropertyValue(object? propertyValue, byte expectedValue)
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetRequiredU8PropertyValue(propertyName, out var actualValue, pluginLogStub);

        Assert.IsTrue(success);
        Assert.AreEqual(expectedValue, actualValue);
    }

    [TestMethod]
    public void TestTryGetRequiredU8PropertyValueWithNull()
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, null } });

        var success = element.TryGetRequiredU8PropertyValue(propertyName, out var _, pluginLogStub);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected property [{propertyName}] to be present: {element}", actualMessage));
    }

    [TestMethod]
    [DataRow(byte.MinValue - 1, JsonValueKind.Number)]
    [DataRow("-1", JsonValueKind.String)]
    [DataRow(byte.MaxValue + 1, JsonValueKind.Number)]
    [DataRow("256", JsonValueKind.String)]
    public void TestTryGetRequiredU8PropertyValueWithInvalidValue(object? propertyValue, JsonValueKind propertyValueKind)
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetRequiredU8PropertyValue(propertyName, out var _, pluginLogStub);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [{propertyValueKind}] value kind to be parsable as [Byte]: {propertyValue}", actualMessage));
    }

    [TestMethod]
    public void TestTryGetRequiredU8PropertyValueWithInvalidKind()
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

        var success = element.TryGetRequiredU8PropertyValue(propertyName, out var _, pluginLogStub);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [False] value kind to be parsable as [Byte]: {propertyValue}", actualMessage));
    }

    [TestMethod]
    [DataRow(0, ushort.MinValue)]
    [DataRow("0", ushort.MinValue)]
    [DataRow(65535, ushort.MaxValue)]
    [DataRow("65535", ushort.MaxValue)]
    public void TestTryGetRequiredU16PropertyValue(object? propertyValue, ushort expectedValue)
    {
        var pluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.NotImplemented };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetRequiredU16PropertyValue(propertyName, out var actualValue, pluginLogStub);

        Assert.IsTrue(success);
        Assert.AreEqual(expectedValue, actualValue);
    }

    [TestMethod]
    public void TestTryGetRequiredU16PropertyValueWithNull()
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, null } });

        var success = element.TryGetRequiredU16PropertyValue(propertyName, out var _, pluginLogStub);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected property [{propertyName}] to be present: {element}", actualMessage));
    }

    [TestMethod]
    [DataRow(ushort.MinValue - 1, JsonValueKind.Number)]
    [DataRow("-1", JsonValueKind.String)]
    [DataRow(ushort.MaxValue + 1, JsonValueKind.Number)]
    [DataRow("65536", JsonValueKind.String)]
    public void TestTryGetRequiredU16PropertyValueWithInvalidValue(object? propertyValue, JsonValueKind propertyValueKind)
    {
        var observer = new StubObserver();

        var pluginLogStub = new StubIPluginLog()
        {
            InstanceBehavior = StubBehaviors.DefaultValue,
            InstanceObserver = observer
        };

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, propertyValue } });

        var success = element.TryGetRequiredU16PropertyValue(propertyName, out var _, pluginLogStub);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [{propertyValueKind}] value kind to be parsable as [UInt16]: {propertyValue}", actualMessage));
    }

    [TestMethod]
    public void TestTryGetRequiredU16PropertyValueWithInvalidKind()
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

        var success = element.TryGetRequiredU16PropertyValue(propertyName, out var _, pluginLogStub);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [False] value kind to be parsable as [UInt16]: {propertyValue}", actualMessage));
    }
}
