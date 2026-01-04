using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Testables;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.Readers.Elements;

[TestClass]
public class TestElementReader : ITestableClassTemp
{
    public TestContext TestContext { get; set; }

    [TestMethod]
    [DataRow("null", JsonValueKind.Null)]
    [DataRow("true", JsonValueKind.True)]
    [DataRow("false", JsonValueKind.False)]
    [DataRow("0", JsonValueKind.Number)]
    [DataRow(""" "" """, JsonValueKind.String)]
    [DataRow("{}", JsonValueKind.Object)]
    [DataRow("[]", JsonValueKind.Array)]
    [DataRow("[0,]", JsonValueKind.Array)]
    public void TestTryReadFromData(string data, JsonValueKind expectedKind)
    {
        var success = new ElementReaderBuilder()
            .Build()
            .TryReadFromData(data, out var element);

        Assert.IsTrue(success);
        Assert.AreEqual(expectedKind, element.ValueKind);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("{")]
    [DataRow("[")]
    public void TestTryReadFromDataWithInvalid(string data)
    {
        var observer = new StubObserver();

        var elementReader = new ElementReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = elementReader.TryReadFromData(data, out var jsonElement);

        Assert.IsFalse(success);
        Assert.AreEqual(default, jsonElement);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Error), 
            actualMessage => Assert.StartsWith("Caught exception while reading [JsonElement] from data", actualMessage));
    }

    [TestMethod]
    [DataRow("null", JsonValueKind.Null)]
    [DataRow("true", JsonValueKind.True)]
    [DataRow("false", JsonValueKind.False)]
    [DataRow("0", JsonValueKind.Number)]
    [DataRow(""" "" """, JsonValueKind.String)]
    [DataRow("{}", JsonValueKind.Object)]
    [DataRow("[]", JsonValueKind.Array)]
    [DataRow("[0,]", JsonValueKind.Array)]
    public void TestTryReadFromFile(string data, JsonValueKind expectedKind)
    {
        var tempDirectory = this.CreateResultsTempDirectory();

        var filePath = Path.Combine(tempDirectory, "File Path");
        File.WriteAllText(filePath, data);

        var success = new ElementReaderBuilder()
            .Build()
            .TryReadFromFile(filePath, out var jsonElement);

        Assert.IsTrue(success);
        Assert.AreEqual(expectedKind, jsonElement.ValueKind);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("{")]
    [DataRow("[")]
    public void TestTryReadFromFileWithInvalid(string data)
    {
        var observer = new StubObserver();

        var tempDirectory = this.CreateResultsTempDirectory();

        var filePath = Path.Combine(tempDirectory, "File Path");
        File.WriteAllText(filePath, data);

        var elementReader = new ElementReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = elementReader.TryReadFromFile(filePath, out var jsonElement);

        Assert.IsFalse(success);
        Assert.AreEqual(default, jsonElement);

        var calls = observer.GetCalls();
        Assert.HasCount(2, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Error),
            actualMessage => Assert.StartsWith("Caught exception while reading [JsonElement] from data", actualMessage));

        AssertPluginLog.MatchObservedCall(calls[1], nameof(IPluginLog.Warning),
            actualMessage => Assert.StartsWith($"Failed to read [JsonElement] from json file [{filePath}]", actualMessage));
    }

    [TestMethod]
    public void TestTryReadFromFileWithMissing()
    {
        var observer = new StubObserver();

        var tempDirectory = this.CreateResultsTempDirectory();

        var filePath = Path.Combine(tempDirectory, "File Path");

        var elementReader = new ElementReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = elementReader.TryReadFromFile(filePath, out var jsonElement);

        Assert.IsFalse(success);
        Assert.AreEqual(default, jsonElement);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Error), 
            actualMessage => Assert.AreEqual($"Caught exception while reading [JsonElement] from json file [{filePath}] (Could not find file '{filePath}'.)", actualMessage));
    }
}
