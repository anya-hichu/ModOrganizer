using ModOrganizer.Configs;
using ModOrganizer.Tests.Configs.Defaults;
using ModOrganizer.Tests.Dalamuds.PluginInterfaces;

namespace ModOrganizer.Tests.Configs.Loaders;

[TestClass]
public class TestConfigLoader
{
    [TestMethod]
    public void TestGet()
    {
        var config = new Config();

        var configLoader = new ConfigLoaderBuild()
            .WithPluginInterfaceConfig(config)
            .Build();

        var actualConfig = configLoader.GetOrDefault();

        Assert.AreSame(config, actualConfig);
    }

    [TestMethod]
    public void TestDefault()
    {
        var config = new Config();

        var configLoader = new ConfigLoaderBuild()
            .WithPluginInterfaceConfig(null)
            .WithConfigDefaultBuild(config)
            .Build();

        var actualConfig = configLoader.GetOrDefault();

        Assert.AreSame(config, actualConfig);

    }
}
