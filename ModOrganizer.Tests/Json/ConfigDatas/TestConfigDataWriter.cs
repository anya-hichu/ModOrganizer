using ModOrganizer.Json.ConfigDatas;
using ModOrganizer.Json.Writers.Clipboards;
using ModOrganizer.Tests.Json.RuleDatas;
using System.Text;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.ConfigDatas;

[TestClass]
public class TestConfigDataWriter
{
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
}
