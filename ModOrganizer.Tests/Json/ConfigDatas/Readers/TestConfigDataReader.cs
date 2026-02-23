using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.ConfigDatas;
using ModOrganizer.Json.ConfigDatas.Readers;
using ModOrganizer.Json.Readers.Clipboards;
using ModOrganizer.Json.Readers.Elements;
using ModOrganizer.Json.Readers.Files;
using ModOrganizer.Json.RuleDatas;
using ModOrganizer.Tests.Json.Readers.Elements;
using ModOrganizer.Tests.Json.RuleDatas;
using ModOrganizer.Tests.TestableClasses;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.ConfigDatas.Readers;

[TestClass]
public class TestConfigDataReader : ITestableClassTemp
{
    public TestContext TestContext { get; set; }

    [TestMethod]
    public void TestTryRead()
    {
        var version = ConfigDataReader.SUPPORTED_VERSION;

        var ruleDatas = Array.Empty<RuleData>();

        var reader = new ConfigDataReaderBuilder()
            .WithRuleDataReaderTryReadMany(ruleDatas)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(ConfigData.Version), version },
            { nameof(ConfigData.Rules), ruleDatas }
        });

        var success = reader.TryRead(element, out var configData);

        Assert.IsTrue(success);
        Assert.IsNotNull(configData);

        Assert.AreEqual(version, configData.Version);
        Assert.AreSame(ruleDatas, configData.Rules);
    }

    [TestMethod]
    public void TestTryReadFromClipboard()
    {
        var observer = new StubObserver();

        var version = ConfigDataReader.SUPPORTED_VERSION;
        var ruleDatas = Array.Empty<RuleData>();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(ConfigData.Version), version },
            { nameof(ConfigData.Rules), ruleDatas }
        });

        var reader = new ConfigDataReaderBuilder()
            .WithRuleDataReaderTryReadMany(ruleDatas)
            .WithElementReaderObserver(observer)
            .WithElementReaderTryReadFromData(element)
            .Build();

        var data = "q1YKSy0qzszPU7Iy0FEKKs1JLVayio6tBQA=";
        var success = reader.TryReadFromClipboard(data, out var configData);

        Assert.IsTrue(success);
        Assert.IsNotNull(configData);

        Assert.AreEqual(version, configData.Version);
        Assert.AreSame(ruleDatas, configData.Rules);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        var call = calls[0];
        Assert.AreEqual(nameof(IElementReader.TryReadFromData), call.StubbedMethod.Name);
        Assert.AreEqual("""{"Version":0,"Rules":[]}""", call.GetArguments()[0] as string);
    }

    [TestMethod]
    public void TestTryReadFromFile()
    {
        var observer = new StubObserver();

        var tempDirectory = this.CreateResultsTempDirectory();
        var filePath = Path.Combine(tempDirectory, "File Path");

        var version = ConfigDataReader.SUPPORTED_VERSION;
        var ruleDatas = Array.Empty<RuleData>();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(ConfigData.Version), version },
            { nameof(ConfigData.Rules), ruleDatas }
        });

        var reader = new ConfigDataReaderBuilder()
            .WithRuleDataReaderTryReadMany(ruleDatas)
            .WithElementReaderObserver(observer)
            .WithElementReaderTryReadFromFile(element)
            .Build();

        var success = reader.TryReadFromFile(filePath, out var configData);

        Assert.IsTrue(success);
        Assert.IsNotNull(configData);

        Assert.AreEqual(version, configData.Version);
        Assert.AreSame(ruleDatas, configData.Rules);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        var call = calls[0];
        Assert.AreEqual(nameof(IElementReader.TryReadFromFile), call.StubbedMethod.Name);
        Assert.AreEqual(filePath, call.GetArguments()[0] as string);
    }
}
