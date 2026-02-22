using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.ConfigDatas;
using ModOrganizer.Json.Readers.Clipboards;
using ModOrganizer.Json.Readers.Elements;
using ModOrganizer.Json.RuleDatas;
using ModOrganizer.Tests.Json.Readers.Elements;
using ModOrganizer.Tests.Json.RuleDatas;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.ConfigDatas;

[TestClass]
public class TestConfigDataReader
{
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
}
