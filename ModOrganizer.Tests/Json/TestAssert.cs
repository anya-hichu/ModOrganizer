using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using System.Text.Json;

namespace ModOrganizer.Tests.Json;

[TestClass]
public class TestAssert
{
    [TestMethod]
    [DataRow("[]", JsonValueKind.Array)]
    [DataRow("false", JsonValueKind.False)]
    [DataRow("null", JsonValueKind.Null)]
    [DataRow("0", JsonValueKind.Number)]
    [DataRow("{}", JsonValueKind.Object)]
    [DataRow(""" "" """, JsonValueKind.String)]
    [DataRow("true", JsonValueKind.True)]
    public void TestIsValue(string data, JsonValueKind kind)
    {
        var assert = new AssertBuilder().Build();

        var jsonElement = JsonSerializer.Deserialize<JsonElement>(data);

        var success = assert.IsValue(jsonElement, kind);

        Assert.IsTrue(success);
    }

    [TestMethod]
    [DataRow("[]", JsonValueKind.Object)]
    [DataRow("false", JsonValueKind.Object)]
    [DataRow("null", JsonValueKind.Object)]
    [DataRow("0", JsonValueKind.Object)]
    [DataRow("{}", JsonValueKind.Null)]
    [DataRow(""" "" """, JsonValueKind.Object)]
    [DataRow("true", JsonValueKind.Object)]
    public void TestIsValueWithMismatch(string data, JsonValueKind kind)
    {
        var observer = new StubObserver();

        var assert = new AssertBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var jsonElement = JsonSerializer.Deserialize<JsonElement>(data);

        var success = assert.IsValue(jsonElement, kind);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), actualMessage => Assert.AreEqual($"Expected value kind [{kind}] but got [{jsonElement.ValueKind}]: {jsonElement}", actualMessage));
    }


    [TestMethod]
    [DataRow("Property Name", " ")]
    [DataRow("Property Name", "value")]
    public void TestIsPropertyPresent(string propertyName, string value)
    {
        var assert = new AssertBuilder().Build();

        var jsonElement = JsonSerializer.SerializeToElement(new Dictionary<string, string>() { { propertyName, value } });

        var success = assert.IsPropertyPresent(jsonElement, propertyName, out var property);

        Assert.IsTrue(success);
        Assert.AreEqual(value, property.GetString());
    }


    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void TestIsPropertyPresentWithMissing(bool warn)
    {
        var observer = new StubObserver();

        var assert = new AssertBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var jsonElement = JsonSerializer.SerializeToElement(new Dictionary<string, string>());

        var propertyName = "Property Name";

        var success = assert.IsPropertyPresent(jsonElement, propertyName, out var _, warn);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();

        if (warn)
        {
            Assert.HasCount(1, calls);
            AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), actualMessage => Assert.AreEqual($"Expected property [{propertyName}] is missing: {jsonElement}", actualMessage));
        }
        else
        {
            Assert.IsEmpty(calls);
        }
    }

    [TestMethod]
    [DataRow("Property Name", " ")]
    [DataRow("Property Name", "value")]
    public void TestIsValuePresent(string propertyName, string value)
    {
        var assert = new AssertBuilder().Build();

        var jsonElement = JsonSerializer.SerializeToElement(new Dictionary<string, string>() { { propertyName, value } });

        var success = assert.IsValuePresent(jsonElement, propertyName, out var actualValue);

        Assert.IsTrue(success);
        Assert.AreEqual(value, actualValue);
    }

    [TestMethod]
    [DataRow("Property Name", null)]
    [DataRow("Property Name", "")]
    public void TestIsValuePresentWithEmpty(string propertyName, string value)
    {
        var observer = new StubObserver();

        var assert = new AssertBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var jsonElement = JsonSerializer.SerializeToElement(new Dictionary<string, string>() { { propertyName, value } });

        var success = assert.IsValuePresent(jsonElement, propertyName, out var _);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();

        Assert.HasCount(1, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), actualMessage => Assert.AreEqual($"Property [{propertyName}] is null or empty: {jsonElement}", actualMessage));
    }

    [TestMethod]
    [DataRow("Property Name", null)]
    [DataRow("Property Name", "")]
    public void TestIsValuePresentWithoutProperty(string propertyName, string value)
    {
        var observer = new StubObserver();

        var assert = new AssertBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var jsonElement = JsonSerializer.SerializeToElement(new Dictionary<string, string>());

        var success = assert.IsValuePresent(jsonElement, propertyName, out var _);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();

        Assert.HasCount(1, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), actualMessage => Assert.AreEqual($"Expected property [{propertyName}] is missing: {jsonElement}", actualMessage));
    }

    [TestMethod]
    public void TestIsValuePresentWithOptionalProperty()
    {
        var observer = new StubObserver();

        var assert = new AssertBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var jsonElement = JsonSerializer.SerializeToElement(new Dictionary<string, string>());

        var success = assert.IsValuePresent(jsonElement, "Property Name", out var _, required: false);

        Assert.IsFalse(success);
        Assert.IsEmpty(observer.GetCalls());
    }


    [TestMethod]
    [DataRow("Property Name", 0, 0)]
    [DataRow("Property Name", "0", 0)]
    [DataRow("Property Name", 255, 255)]
    [DataRow("Property Name", "255", 255)]
    public void TestIsU8ValueWithSuccess(string propertyName, object value, int expectedValue)
    {
        var assert = new AssertBuilder().Build();

        var jsonElement = JsonSerializer.SerializeToElement(new Dictionary<string, object>() { { propertyName, value } });

        var success = assert.IsU8Value(jsonElement, propertyName, out var actualValue);

        Assert.IsTrue(success);
        Assert.AreEqual(expectedValue, actualValue);
    }

    [TestMethod]
    [DataRow("Property Name", -1)]
    [DataRow("Property Name", "-1")]
    [DataRow("Property Name", 256)]
    [DataRow("Property Name", "256")]
    public void TestIsU8ValueWithInvalid(string propertyName, object value)
    {
        var observer = new StubObserver();

        var assert = new AssertBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var jsonElement = JsonSerializer.SerializeToElement(new Dictionary<string, object>() { { propertyName, value } });

        var success = assert.IsU8Value(jsonElement, propertyName, out var _);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();

        Assert.HasCount(1, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), actualMessage => Assert.AreEqual($"Property [{propertyName}] is not parsable as [Byte]: {jsonElement}", actualMessage));
    }

    [TestMethod]
    public void TestIsU8ValueWithoutProperty()
    {
        var observer = new StubObserver();

        var assert = new AssertBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var jsonElement = JsonSerializer.SerializeToElement(new Dictionary<string, object>());

        var propertyName = "Property Name";

        var success = assert.IsU8Value(jsonElement, propertyName, out var _);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();

        Assert.HasCount(1, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), actualMessage => Assert.AreEqual($"Expected property [{propertyName}] is missing: {jsonElement}", actualMessage));
    }

    [TestMethod]
    public void TestIsU8ValueWithOptionalProperty()
    {
        var observer = new StubObserver();

        var assert = new AssertBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var jsonElement = JsonSerializer.SerializeToElement(new Dictionary<string, object>());

        var success = assert.IsU8Value(jsonElement, "Property Name", out var _, required: false);

        Assert.IsFalse(success);
        Assert.IsEmpty(observer.GetCalls());
    }

    [TestMethod]
    [DataRow("Property Name", 0, 0)]
    [DataRow("Property Name", "0", 0)]
    [DataRow("Property Name", 65535, 65535)]
    [DataRow("Property Name", "65535", 65535)]
    public void TestIsU16ValueWithSuccess(string propertyName, object value, int expectedValue)
    {
        var assert = new AssertBuilder().Build();

        var jsonElement = JsonSerializer.SerializeToElement(new Dictionary<string, object>() { { propertyName, value } });

        var success = assert.IsU16Value(jsonElement, propertyName, out var actualValue);

        Assert.IsTrue(success);
        Assert.AreEqual(expectedValue, actualValue);
    }

    [TestMethod]
    [DataRow("Property Name", -1)]
    [DataRow("Property Name", "-1")]
    [DataRow("Property Name", 65536)]
    [DataRow("Property Name", "65536")]
    public void TestIsU16ValueWithInvalid(string propertyName, object value)
    {
        var observer = new StubObserver();

        var assert = new AssertBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var jsonElement = JsonSerializer.SerializeToElement(new Dictionary<string, object>() { { propertyName, value } });

        var success = assert.IsU16Value(jsonElement, propertyName, out var _);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();

        Assert.HasCount(1, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), actualMessage => Assert.AreEqual($"Property [{propertyName}] is not parsable as [UInt16]: {jsonElement}", actualMessage));
    }

    [TestMethod]
    public void TestIsU16ValueWithoutProperty()
    {
        var observer = new StubObserver();

        var assert = new AssertBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var jsonElement = JsonSerializer.SerializeToElement(new Dictionary<string, object>());

        var propertyName = "Property Name";

        var success = assert.IsU16Value(jsonElement, propertyName, out var _);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();

        Assert.HasCount(1, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), actualMessage => Assert.AreEqual($"Expected property [{propertyName}] is missing: {jsonElement}", actualMessage));
    }

    [TestMethod]
    public void TestIsU16ValueWithOptionalProperty()
    {
        var observer = new StubObserver();

        var assert = new AssertBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var jsonElement = JsonSerializer.SerializeToElement(new Dictionary<string, object>());

        var success = assert.IsU16Value(jsonElement, "Property Name", out var _, required: false);

        Assert.IsFalse(success);
        Assert.IsEmpty(observer.GetCalls());
    }

    [TestMethod]
    public void TestIsStringDict()
    {
        var assert = new AssertBuilder().Build();

        var propertyName = "Property Name";
        var value = "Value";

        var jsonElement = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, value } });

        var success = assert.IsStringDict(jsonElement, out var actualDict);

        Assert.IsTrue(success);
        Assert.IsNotNull(actualDict);
        Assert.AreEqual(value, actualDict[propertyName]);
    }

    [TestMethod]
    public void TestIsStringDictWithEmpty()
    {
        var assert = new AssertBuilder().Build();

        var jsonElement = JsonSerializer.SerializeToElement(new Dictionary<string, object?>());

        var success = assert.IsStringDict(jsonElement, out var actualDict);

        Assert.IsTrue(success);
        Assert.IsNotNull(actualDict);
        Assert.IsEmpty(actualDict);
    }

    [TestMethod]
    public void TestIsStringDictWithInvalid()
    {
        var observer = new StubObserver();

        var assert = new AssertBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var propertyName = "Property Name";

        var jsonElement = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { propertyName, null } });

        var success = assert.IsStringDict(jsonElement, out var actualDict);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();

        Assert.HasCount(1, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), actualMessage => Assert.AreEqual($"Expected value kind [String] but got [Null]: ", actualMessage));
    }

    [TestMethod]
    public void TestIsStringArray()
    {
        var assert = new AssertBuilder().Build();

        var value = "value";
        var jsonElement = JsonSerializer.SerializeToElement(new List<object?>() { value });

        var success = assert.IsStringArray(jsonElement, out var actualArray);

        Assert.IsTrue(success);
        Assert.IsNotNull(actualArray);
        Assert.HasCount(1, actualArray);
        Assert.AreEqual(value, actualArray[0]);
    }

    [TestMethod]
    public void TestIsStringArrayWithEmpty()
    {
        var assert = new AssertBuilder().Build();

        var jsonElement = JsonSerializer.SerializeToElement(Array.Empty<object?>());

        var success = assert.IsStringArray(jsonElement, out var actualArray);

        Assert.IsTrue(success);
        Assert.IsNotNull(actualArray);
        Assert.IsEmpty(actualArray);
    }

    [TestMethod]
    public void TestIsStringArrayWithInvalid()
    {
        var observer = new StubObserver();

        var assert = new AssertBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var jsonElement = JsonSerializer.SerializeToElement(new List<object?>() { null });

        var success = assert.IsStringArray(jsonElement, out var actualArray);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();

        Assert.HasCount(1, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), actualMessage => Assert.AreEqual($"Expected value kind [String] but got [Null]: ", actualMessage));
    }

    [TestMethod]
    [DataRow(int.MinValue)]
    [DataRow(int.MaxValue)]
    public void TestIsIntArray(int value)
    {
        var assert = new AssertBuilder().Build();;
        var jsonElement = JsonSerializer.SerializeToElement(new List<object?>() { value });

        var success = assert.IsIntArray(jsonElement, out var actualArray);

        Assert.IsTrue(success);
        Assert.IsNotNull(actualArray);
        Assert.HasCount(1, actualArray);
        Assert.AreEqual(value, actualArray[0]);
    }

    [TestMethod]
    public void TestIsIntArrayWithEmpty()
    {
        var assert = new AssertBuilder().Build();

        var jsonElement = JsonSerializer.SerializeToElement(Array.Empty<object?>());

        var success = assert.IsIntArray(jsonElement, out var actualArray);

        Assert.IsTrue(success);
        Assert.IsNotNull(actualArray);
        Assert.IsEmpty(actualArray);
    }

    [TestMethod]
    public void TestIsIntArrayWithInvalid()
    {
        var observer = new StubObserver();

        var assert = new AssertBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var jsonElement = JsonSerializer.SerializeToElement(new List<object?>() { null });

        var success = assert.IsIntArray(jsonElement, out var actualArray);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();

        Assert.HasCount(1, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), actualMessage => Assert.AreEqual($"Expected value kind [Number] but got [Null]: ", actualMessage));
    }
}
