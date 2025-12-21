using ModOrganizer.Tests.Shared;

namespace ModOrganizer.Tests;

[TestClass]
public sealed class TestPlugin
{
    public TestContext TestContext { get; set; } = null!;

    [TestMethod]
    public void TestNew()
    {
        var tempDirectory = TestContext.CreateTestTempDirectory();

        var stubs = new TestPluginStubs(tempDirectory);
        Plugin.PluginInterface = stubs.PluginInterfaceStub;
        Plugin.CommandManager = stubs.CommandManagerStub;
        Plugin.PluginLog = stubs.PluginLogStub;

        var plugin = new Plugin();
        Assert.IsNotNull(plugin);
    }
}
