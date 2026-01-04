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
        var elementReader = new ElementReaderBuilder().Build();

        var success = elementReader.TryReadFromData(data, out var element);

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
            actualMessage => Assert.StartsWith($"Caught exception while reading [JsonElement] from data", actualMessage));
    }

    [TestMethod]
    public void TestTryReadFromFile()
    {
        var tempDirectory = this.CreateResultsTempDirectory();

        var filePath = Path.Combine(tempDirectory, "FilePath.json");
        File.WriteAllText(filePath, "{}");

        var elementReader = new ElementReaderBuilder().Build();

        var success = elementReader.TryReadFromFile(filePath, out var jsonElement);

        Assert.IsTrue(success);
        Assert.AreEqual(JsonValueKind.Object, jsonElement.ValueKind);
    }

    [TestMethod]
    public void TestTryReadFromFileWithMissing()
    {
        var observer = new StubObserver();

        var tempDirectory = this.CreateResultsTempDirectory();

        var filePath = Path.Combine(tempDirectory, "FilePath.json");

        var elementReader = new ElementReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var success = elementReader.TryReadFromFile(filePath, out var jsonElement);

        Assert.IsFalse(success);
        Assert.AreEqual(default, jsonElement);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Error), actualMessage => Assert.AreEqual($"Caught exception while reading [JsonElement] from json file [{filePath}] (Could not find file '{filePath}'.)", actualMessage));
    }
}
