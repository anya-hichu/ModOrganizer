using ModOrganizer.Configs;

namespace ModOrganizer.Tests.Configs;

[TestClass]
public class TestConfigBuilder : TestClass
{
    [TestMethod]
    public void TestBuildDefault()
    {
        var config = ConfigBuilder.BuildDefault();

        Assert.AreEqual(0, config.Version);
        Assert.HasCount(16, config.Rules);

        Assert.IsFalse(config.AutoProcessEnabled);
        Assert.AreEqual(1000, config.AutoProcessDelayMs);

        Assert.IsEmpty(config.Backups);
        Assert.IsTrue(config.AutoBackupEnabled);
        Assert.AreEqual(10, config.AutoBackupLimit);
    }
}
