using ModOrganizer.Tests.Mods.ModInterops;
using ModOrganizer.Tests.TestableClasses;

namespace ModOrganizer.Tests.Mods.ModFileSystems;

[TestClass]
public class TestModFileSystem : ITestableClassTemp
{
    public TestContext TestContext { get; set; }

    [TestMethod]
    public void TestGetRootFolderWithNone()
    {
        var modFileSystem = new ModFileSystemBuilder()
            .WithModInteropGetModList([])
            .Build();

        var rootFolder = modFileSystem.GetRootFolder();

        Assert.IsEmpty(rootFolder.Files);
        Assert.IsEmpty(rootFolder.Folders);
    }

    [TestMethod]
    public void TestGetRootFolderWithSingle()
    {
        var modDirectory = "Mod Directory";
        var modName = "Mod Name";

        var modList = new Dictionary<string, string>()
        {
            { modDirectory, modName }
        };

        var folderName = "Folder Name";
        var modPath = $"{folderName}/File Name";

        var modFileSystem = new ModFileSystemBuilder()
            .WithModInteropGetModList(modList)
            .WithModInteropGetModPath(modPath)
            .Build();

        var rootFolder = modFileSystem.GetRootFolder();

        Assert.IsEmpty(rootFolder.Files);
        Assert.HasCount(1, rootFolder.Folders);

        var folder = rootFolder.Folders.ElementAt(0);

        Assert.AreEqual(folderName, folder.Name);

        Assert.HasCount(1, folder.Files);
        Assert.IsEmpty(folder.Folders);

        var file = folder.Files.ElementAt(0);

        Assert.AreEqual(modPath, file.Path);

        Assert.AreEqual(modDirectory, file.Directory);
        Assert.AreEqual(modName, file.Name);
    }

    [TestMethod]
    public void TestGetRootFolderWithMultiple()
    {
        var firstModDirectory = "Mod Directory 1";
        var secondModDirectory = "Mod Directory 2";

        var modName = "Mod Name";

        var modList = new Dictionary<string, string>()
        {
            { firstModDirectory, modName },
            { secondModDirectory, modName }
        };

        var modFileSystem = new ModFileSystemBuilder()
            .WithModInteropGetModList(modList)
            .WithModInteropGetModPath(modDirectory => $"Folder Name/{modDirectory}")
            .Build();

        var rootDirectory = modFileSystem.GetRootFolder();

        Assert.IsEmpty(rootDirectory.Files);
        var folders = rootDirectory.Folders;
        Assert.HasCount(1, folders);

        var folder = folders.ElementAt(0);
        Assert.AreEqual("Folder Name", folder.Name);

        var subfiles = folder.Files;
        Assert.HasCount(2, subfiles);
        Assert.IsEmpty(folder.Folders);

        var firstFile = subfiles.ElementAt(0);
        Assert.AreEqual("Folder Name/Mod Directory 1", firstFile.Path);
        Assert.AreEqual(firstModDirectory, firstFile.Directory);
        Assert.AreEqual(modName, firstFile.Name);

        var secondFile = subfiles.ElementAt(1);
        Assert.AreEqual("Folder Name/Mod Directory 2", secondFile.Path);
        Assert.AreEqual(secondModDirectory, secondFile.Directory);
        Assert.AreEqual(modName, secondFile.Name);
    }

    [TestMethod]
    public void TestInvalidateRootFolderCacheOnModsChanged()
    {
        var builder = new ModFileSystemBuilder();

        var modList = new Dictionary<string, string>();

        var modFileSystem = builder
            .WithModInteropGetModList(modList)
            .WithModInteropGetModPath("Mod Path")
            .Build();

        var rootFolder = modFileSystem.GetRootFolder();

        modList.Add("Mod Directory", "Mod Name");
        Assert.IsEmpty(rootFolder.Files);

        builder.ModInteropStub.OnModsChangedEvent();

        var newRootFolder = modFileSystem.GetRootFolder();
        Assert.HasCount(1, newRootFolder.Files);
    }
}
