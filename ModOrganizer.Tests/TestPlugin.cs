using ModOrganizer.Tests.Dalamuds.CommandManagers;
using ModOrganizer.Tests.Dalamuds.PenumbraApis;
using ModOrganizer.Tests.Dalamuds.PluginInterfaces;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Testables;
using Penumbra.Api.Enums;

namespace ModOrganizer.Tests;

[TestClass]
public sealed class TestPlugin : ITestableClassTemp
{
    public TestContext TestContext { get; set; }

    [TestMethod]
    public void TestBuild()
    {
        var tempDirectory = this.CreateResultsTempDirectory();

        var configDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer)));
        var penumbraConfigDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(Penumbra)));

        Directory.CreateDirectory(Path.Combine(penumbraConfigDirectory.FullName, "mod_data"));

        var modsDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, "ModsDirectory"));

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
