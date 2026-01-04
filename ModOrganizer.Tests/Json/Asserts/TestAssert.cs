using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.Asserts;

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
        var element = JsonSerializer.Deserialize<JsonElement>(data);

        var success = new AssertBuilder()
            .Build()
            .IsValue(element, kind);

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

        var element = JsonSerializer.Deserialize<JsonElement>(data);

        var success = assert.IsValue(element, kind);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), 
            actualMessage => Assert.AreEqual($"Expected value kind [{kind}] but found [{element.ValueKind}]: {element}", actualMessage));
    }


    [TestMethod]
    [DataRow(" ")]
    [DataRow("value")]
    public void TestIsPropertyPresent(object? value)
    {
        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() 
        { 
            { propertyName, value } 
        });

        var success = new AssertBuilder()
            .Build()
            .IsPropertyPresent(element, propertyName, out var property);

        Assert.IsTrue(success);
        Assert.AreEqual(value, property.GetString());
    }


    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void TestIsPropertyPresentWithMissing(bool warn)
    {
        var observer = new StubObserver();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>());

        var propertyName = "Property Name";

        var assert = new AssertBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = assert.IsPropertyPresent(element, propertyName, out var _, warn);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();

        if (warn)
        {
            Assert.HasCount(1, calls);
            AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
                actualMessage => Assert.AreEqual($"Expected property [{propertyName}] is missing: {element}", actualMessage));
        }
        else
        {
            Assert.IsEmpty(calls);
        }
    }

    [TestMethod]
    [DataRow(" ")]
    [DataRow("value")]
    public void TestIsValuePresent(object? value)
    {
        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        { 
            { propertyName, value } 
        });

        var success = new AssertBuilder()
            .Build()
            .IsValuePresent(element, propertyName, out var actualValue);

        Assert.IsTrue(success);
        Assert.AreEqual(value, actualValue);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    public void TestIsValuePresentWithEmpty(object? value)
    {
        var observer = new StubObserver();

        var propertyName = "Property Name";
        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() 
        { 
            { propertyName, value } 
        });

        var assert = new AssertBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = assert.IsValuePresent(element, propertyName, out var _);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();

        Assert.HasCount(1, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), 
            actualMessage => Assert.AreEqual($"Property [{propertyName}] is null or empty: {element}", actualMessage));
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    public void TestIsValuePresentWithoutProperty(object? value)
    {
        var observer = new StubObserver();

        var assert = new AssertBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>());

        var propertyName = "Property Name";
        var success = assert.IsValuePresent(element, propertyName, out var _);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();

        Assert.HasCount(1, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), 
            actualMessage => Assert.AreEqual($"Expected property [{propertyName}] is missing: {element}", actualMessage));
    }

    [TestMethod]
    public void TestIsValuePresentWithOptionalProperty()
    {
        var observer = new StubObserver();

        var assert = new AssertBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>());

        var success = assert.IsValuePresent(element, "Property Name", out var _, required: false);

        Assert.IsFalse(success);
        Assert.IsEmpty(observer.GetCalls());
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(null)]
    public void TestIsValueOptional(object? value)
    {
        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { propertyName, value }
        });

        var success = new AssertBuilder()
            .Build()
            .IsOptionalValue(element, propertyName, out var actualValue);

        Assert.IsTrue(success);
        Assert.AreEqual(value, actualValue);
    }

    [TestMethod]
    public void TestIsValueOptionalWithMissing()
    {
        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>());

        var propertyName = "Property Name";

        var success = new AssertBuilder()
            .Build()
            .IsOptionalValue(element, propertyName, out var actualValue);

        Assert.IsTrue(success);
        Assert.IsNull(actualValue);
    }

    [TestMethod]
    [DataRow(0, JsonValueKind.Number)]
    [DataRow(false, JsonValueKind.False)]
    public void TestIsValueOptionalWithInvalid(object? value, JsonValueKind kind)
    {
        var observer = new StubObserver();

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { propertyName, value }
        });

        var assert = new AssertBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = assert.IsOptionalValue(element, propertyName, out var actualValue);

        Assert.IsFalse(success);
        Assert.IsNull(actualValue);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), 
            actualMessage => Assert.AreEqual($"Expected property [Name] value kind to be [String] or [Null] but found [{kind}]: {element}", actualMessage));
    }

    [TestMethod]
    [DataRow(0, 0)]
    [DataRow("0", 0)]
    [DataRow(255, 255)]
    [DataRow("255", 255)]
    public void TestIsU8ValueWithSuccess(object? value, int expectedValue)
    {
        var propertyName = "Property Name";
        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() 
        { 
            { propertyName, value } 
        });

        var success = new AssertBuilder()
            .Build()
            .IsU8Value(element, propertyName, out var actualValue);

        Assert.IsTrue(success);
        Assert.AreEqual(expectedValue, actualValue);
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow("-1")]
    [DataRow(256)]
    [DataRow("256")]
    public void TestIsU8ValueWithInvalid(object? value)
    {
        var observer = new StubObserver();

        var propertyName = "Property Name";
        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() 
        { 
            { propertyName, value } 
        });

        var assert = new AssertBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = assert.IsU8Value(element, propertyName, out var _);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();

        Assert.HasCount(1, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), 
            actualMessage => Assert.AreEqual($"Property [{propertyName}] is not parsable as [Byte]: {element}", actualMessage));
    }

    [TestMethod]
    public void TestIsU8ValueWithoutProperty()
    {
        var observer = new StubObserver();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>());

        var propertyName = "Property Name";

        var assert = new AssertBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = assert.IsU8Value(element, propertyName, out var _);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();

        Assert.HasCount(1, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), 
            actualMessage => Assert.AreEqual($"Expected property [{propertyName}] is missing: {element}", actualMessage));
    }

    [TestMethod]
    public void TestIsU8ValueWithOptionalProperty()
    {
        var observer = new StubObserver();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>());

        var assert = new AssertBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = assert.IsU8Value(element, "Property Name", out var _, required: false);

        Assert.IsFalse(success);
        Assert.IsEmpty(observer.GetCalls());
    }

    [TestMethod]
    [DataRow(0, 0)]
    [DataRow("0", 0)]
    [DataRow(65535, 65535)]
    [DataRow("65535", 65535)]
    public void TestIsU16ValueWithSuccess(object? value, int expectedValue)
    {
        var propertyName = "Property Name";
        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() 
        { 
            { propertyName, value } 
        });

        var success = new AssertBuilder()
            .Build()
            .IsU16Value(element, propertyName, out var actualValue);

        Assert.IsTrue(success);
        Assert.AreEqual(expectedValue, actualValue);
    }

    [TestMethod]
    [DataRow( -1)]
    [DataRow("-1")]
    [DataRow(65536)]
    [DataRow("65536")]
    public void TestIsU16ValueWithInvalid(object? value)
    {
        var observer = new StubObserver();

        var propertyName = "Property Name";
        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() 
        { 
            { propertyName, value } 
        });

        var assert = new AssertBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = assert.IsU16Value(element, propertyName, out var _);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();

        Assert.HasCount(1, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), actualMessage => Assert.AreEqual($"Property [{propertyName}] is not parsable as [UInt16]: {element}", actualMessage));
    }

    [TestMethod]
    public void TestIsU16ValueWithoutProperty()
    {
        var observer = new StubObserver();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>());

        var propertyName = "Property Name";
        
        var assert = new AssertBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = assert.IsU16Value(element, propertyName, out var _);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();

        Assert.HasCount(1, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), 
            actualMessage => Assert.AreEqual($"Expected property [{propertyName}] is missing: {element}", actualMessage));
    }

    [TestMethod]
    public void TestIsU16ValueWithOptionalProperty()
    {
        var observer = new StubObserver();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>());

        var assert = new AssertBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = assert.IsU16Value(element, "Property Name", out var _, required: false);

        Assert.IsFalse(success);
        Assert.IsEmpty(observer.GetCalls());
    }

    [TestMethod]
    public void TestIsStringDict()
    {
        var propertyName = "Property Name";
        var value = "Value";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() 
        { 
            { propertyName, value } 
        });

        var success = new AssertBuilder()
            .Build()
            .IsStringDict(element, out var actualDict);

        Assert.IsTrue(success);
        Assert.IsNotNull(actualDict);
        Assert.AreEqual(value, actualDict[propertyName]);
    }

    [TestMethod]
    public void TestIsStringDictWithEmpty()
    {
        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>());

        var success = new AssertBuilder()
            .Build()
            .IsStringDict(element, out var actualDict);

        Assert.IsTrue(success);
        Assert.IsNotNull(actualDict);
        Assert.IsEmpty(actualDict);
    }

    [TestMethod]
    public void TestIsStringDictWithInvalid()
    {
        var observer = new StubObserver();

        var propertyName = "Property Name";

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() 
        { 
            { propertyName, null } 
        });

        var assert = new AssertBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = assert.IsStringDict(element, out var actualDict);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();

        Assert.HasCount(1, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), 
            actualMessage => Assert.AreEqual($"Expected value kind [String] but found [Null]: ", actualMessage));
    }

    [TestMethod]
    public void TestIsStringArray()
    {
        var value = "value";
        var element = JsonSerializer.SerializeToElement(new List<object?>() { value });

        var success = new AssertBuilder()
            .Build()
            .IsStringArray(element, out var actualArray);

        Assert.IsTrue(success);
        Assert.IsNotNull(actualArray);
        Assert.HasCount(1, actualArray);
        Assert.AreEqual(value, actualArray[0]);
    }

    [TestMethod]
    public void TestIsStringArrayWithEmpty()
    {
        var element = JsonSerializer.SerializeToElement(Array.Empty<object?>());

        var success = new AssertBuilder()
            .Build()
            .IsStringArray(element, out var actualArray);

        Assert.IsTrue(success);
        Assert.IsNotNull(actualArray);
        Assert.IsEmpty(actualArray);
    }

    [TestMethod]
    public void TestIsStringArrayWithInvalid()
    {
        var observer = new StubObserver();

        var element = JsonSerializer.SerializeToElement(new List<object?>() { null });

        var assert = new AssertBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = assert.IsStringArray(element, out var actualArray);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();

        Assert.HasCount(1, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), 
            actualMessage => Assert.AreEqual($"Expected value kind [String] but found [Null]: ", actualMessage));
    }

    [TestMethod]
    [DataRow(int.MinValue)]
    [DataRow(int.MaxValue)]
    public void TestIsIntArray(int value)
    {
        var element = JsonSerializer.SerializeToElement(new List<object?>() { value });

        var success = new AssertBuilder()
            .Build()
            .IsIntArray(element, out var array);

        Assert.IsTrue(success);
        Assert.IsNotNull(array);
        Assert.HasCount(1, array);
        Assert.AreEqual(value, array[0]);
    }

    [TestMethod]
    public void TestIsIntArrayWithEmpty()
    {
        var element = JsonSerializer.SerializeToElement(Array.Empty<object?>());

        var success = new AssertBuilder()
            .Build()
            .IsIntArray(element, out var array);

        Assert.IsTrue(success);
        Assert.IsNotNull(array);
        Assert.IsEmpty(array);
    }

    [TestMethod]
    public void TestIsIntArrayWithInvalid()
    {
        var observer = new StubObserver();

        var element = JsonSerializer.SerializeToElement(new List<object?>() { null });

        var assert = new AssertBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = assert.IsIntArray(element, out var actualArray);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();

        Assert.HasCount(1, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning), 
            actualMessage => Assert.AreEqual($"Expected value kind [Number] but found [Null]: ", actualMessage));
    }
}
