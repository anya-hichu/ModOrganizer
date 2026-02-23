using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.ConfigDatas;
using ModOrganizer.Json.Writers.Clipboards;
using ModOrganizer.Json.Writers.Files;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Json.RuleDatas;
using ModOrganizer.Tests.TestableClasses;
using System.Text;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.ConfigDatas;

[TestClass]
public class TestConfigDataWriter : ITestableClassTemp
{
    public TestContext TestContext { get; set; }

    [TestMethod]
    public void TestTryWrite()
    {
        var configData = new ConfigData()
        {
            Version = ConfigDataReader.SUPPORTED_VERSION,
            Rules = []
        };

        var writer = new ConfigDataWriterBuilder()
            .WithRuleDataWriterTryWriteMany("[]")
            .Build();

        using var stream = new MemoryStream();

        using (var jsonWriter = new Utf8JsonWriter(stream))
        {
            var success = writer.TryWrite(jsonWriter, configData);
            Assert.IsTrue(success);
        }

        var content = Encoding.UTF8.GetString(stream.ToArray());
        Assert.AreEqual("""{"Version":0,"Rules":[]}""", content);
    }

    [TestMethod]
    public void TestTryWriteToClipboard()
    {
        var configData = new ConfigData()
        {
            Version = ConfigDataReader.SUPPORTED_VERSION,
            Rules = []
        };

        var writer = new ConfigDataWriterBuilder()
            .WithRuleDataWriterTryWriteMany("[]")
            .Build();

        var success = writer.TryWriteToClipboard(configData, out var data);

        Assert.IsTrue(success);
        Assert.AreEqual("q1YKSy0qzszPU7Iy0FEKKs1JLVayio6tBQA=", data);
    }

    [TestMethod]
    public void TestTryWriteToClipboardWithInvalidRuleDatas()
    {
        var configData = new ConfigData()
        {
            Version = ConfigDataReader.SUPPORTED_VERSION,
            Rules = []
        };

        var writer = new ConfigDataWriterBuilder()
            .WithRuleDataWriterTryWriteMany(null)
            .Build();

        var success = writer.TryWriteToClipboard(configData, out var data);

        Assert.IsFalse(success);
        Assert.IsNull(data);
    }

    [TestMethod]
    public void TestTryWriteToClipboardWithException()
    {
        var observer = new StubObserver();

        var configData = new ConfigData()
        {
            Version = ConfigDataReader.SUPPORTED_VERSION,
            Rules = []
        };

        var writer = new ConfigDataWriterBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithRuleDataWriterTryWriteMany("[")
            .Build();

        var success = writer.TryWriteToClipboard(configData, out var data);

        Assert.IsFalse(success);
        Assert.IsNull(data);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Error),
            actualMessage => Assert.AreEqual("Caught exception while writing [ConfigData] to clipboard data ('}' is invalid following a property name.)", actualMessage));
    }

    [TestMethod]
    public void TestTryWriteToFile()
    {
        var tempDirectory = this.CreateResultsTempDirectory();

        var filePath = Path.Combine(tempDirectory, "File Path");

        var configData = new ConfigData()
        {
            Version = ConfigDataReader.SUPPORTED_VERSION,
            Rules = []
        };

        var writer = new ConfigDataWriterBuilder()
            .WithRuleDataWriterTryWriteMany("[]")
            .Build();

        var success = writer.TryWriteToFile(filePath, configData);

        Assert.IsTrue(success);
        Assert.AreEqual("""{"Version":0,"Rules":[]}""", File.ReadAllText(filePath));
    }

    [TestMethod]
    public void TestTryWriteToFileWithInvalidRuleDatas()
    {
        var tempDirectory = this.CreateResultsTempDirectory();

        var filePath = Path.Combine(tempDirectory, "File Path");

        var configData = new ConfigData()
        {
            Version = ConfigDataReader.SUPPORTED_VERSION,
            Rules = []
        };

        var writer = new ConfigDataWriterBuilder()
            .WithRuleDataWriterTryWriteMany(null)
            .Build();

        var success = writer.TryWriteToFile(filePath, configData);

        Assert.IsFalse(success);
        Assert.IsFalse(File.Exists(filePath));
    }

    [TestMethod]
    public void TestTryWriteToFileWithException()
    {
        var observer = new StubObserver();

        var tempDirectory = this.CreateResultsTempDirectory();

        var filePath = Path.Combine(tempDirectory, "File Path");

        var configData = new ConfigData()
        {
            Version = ConfigDataReader.SUPPORTED_VERSION,
            Rules = []
        };

        var writer = new ConfigDataWriterBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithRuleDataWriterTryWriteMany("[")
            .Build();

        var success = writer.TryWriteToFile(filePath, configData);

        Assert.IsFalse(success);
        Assert.IsFalse(File.Exists(filePath));

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Error),
            actualMessage => Assert.AreEqual("Caught exception while writing [ConfigData] to file ('}' is invalid following a property name.)", actualMessage));
    }
}
