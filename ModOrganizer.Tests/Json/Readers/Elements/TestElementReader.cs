using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.Readers.Elements;

[TestClass]
public class TestElementReader
{
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

        var success = elementReader.TryReadFromData(data, out var element);

        Assert.IsFalse(success);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Error), 
            actualMessage => Assert.StartsWith($"Caught exception while reading [JsonElement] from data", actualMessage));
    }
}
