using ModOrganizer.Tests.Shared.CommandManager;
using ModOrganizer.Tests.Shared.PenumbraApis;
using ModOrganizer.Tests.Shared.PluginInterfaces;
using ModOrganizer.Tests.Shared.PluginLogs;
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

        var modsDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, "Mods"));

        using var plugin = new PluginBuilder()
            .WithPluginLogDefaults()
            .WithCommandManagerAddHandler(true)
            .WithCommandManagerRemoveHandler(true)
            .WithPluginInterfaceConfig(null)
            .WithPluginInterfaceConfigDirectory(configDirectory)
            .WithPluginInterfaceInjectObject(false)
            .WithPluginInterfaceUiBuilderStub()
            .WithPenumbraApiGetModDirectory(modsDirectory)
            .WithPenumbraApiGetModList([])
            .WithPenumbraApiGetChangedItems([])
            .WithPenumbraApiSetModPath(PenumbraApiEc.Success)
            .WithPenumbraApiModAddedOrDeletedNoop()
            .WithPenumbraApiModMovedNoop()
            .WithPenumbraApiModDirectoryChangedNoop()
            .WithPenumbraApiRegisterOrUnregisterSettingsSection(PenumbraApiEc.Success)
            .Build();

        Assert.IsNotNull(plugin);
    }
}
