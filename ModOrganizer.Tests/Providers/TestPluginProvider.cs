using Dalamud.Plugin;
using ModOrganizer.Tests.Dalamuds.PluginInterfaces;

namespace ModOrganizer.Tests.Providers;

[TestClass]
public class TestPluginProvider
{
    [TestMethod]
    public void TestGet()
    {
        var builder = new PluginProviderBuilder()
            .WithPluginInterfaceInjectObject(false);

        using var pluginProvider = builder.Build();

        var pluginInterface = pluginProvider.Get<IDalamudPluginInterface>();

        Assert.AreSame(pluginInterface, builder.PluginInterfaceStub);
    }

    [TestMethod]
    public void TestDisposeWithoutCache()
    {
        var pluginProvider = new PluginProviderBuilder()
            .WithPluginInterfaceInjectObject(false)
            .Build();

        pluginProvider.Dispose();
    }
}
