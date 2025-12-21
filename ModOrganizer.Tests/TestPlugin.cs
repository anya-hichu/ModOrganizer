namespace ModOrganizer.Tests;

[TestClass]
public sealed class TestPlugin : TestClass
{
    [TestMethod]
    public void TestNew()
    {
        var tempDirectory = CreateResultsTempDirectory();

        var fakes = new PluginFakes(tempDirectory);
        Plugin.PluginInterface = fakes.PluginInterface;
        Plugin.CommandManager = fakes.CommandManager;
        Plugin.PluginLog = fakes.PluginLog;

        var plugin = new Plugin();

        Assert.IsNotNull(plugin);
    }
}
