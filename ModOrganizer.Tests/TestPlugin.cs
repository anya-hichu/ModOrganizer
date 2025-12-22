using Penumbra.Api.Enums;

namespace ModOrganizer.Tests;

[TestClass]
public sealed class TestPlugin : TestClass
{
    [TestMethod]
    public void TestBuild()
    {
        var tempDirectory = CreateResultsTempDirectory();

        var configDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer)));
        var penumbraConfigDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(Penumbra)));

        Directory.CreateDirectory(Path.Combine(penumbraConfigDirectory.FullName, "mod_data"));

        var modDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, "Mods"));

        using var plugin = new PluginBuilder()
            .WithPluginLogDefaults()
            .WithPluginInterfaceConfig(null)
            .WithPluginInterfaceConfigDirectory(configDirectory)
            .WithPluginInterfaceInjectObjectFalse()
            .WithCommandManagerAddHandlerTrue()
            .WithCommandManagerRemoveHandlerTrue()
            .WithPenumbraModDirectory(modDirectory)
            .WithPenumbraModList([])
            .WithPenumbraChangedItems([])
            .WithPenumbraSetModPath(PenumbraApiEc.Success)
            .WithPenumbraModAddedOrDeletedNoop()
            .WithPenumbraModMovedNoop()
            .WithPenumbraModDirectoryChangedNoop()
            .WithPenumbraRegisterOrUnregisterSettingSectionNoop()
            .Build();

        Assert.IsNotNull(plugin);
    }
}
