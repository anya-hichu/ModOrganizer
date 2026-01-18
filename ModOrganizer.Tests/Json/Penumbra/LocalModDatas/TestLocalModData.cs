using ModOrganizer.Json.Penumbra.LocalModDatas;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.Penumbra.LocalModDatas;

[TestClass]
public class TestLocalModData
{
    [TestMethod]
    public void TestTryRead()
    {
        var localTags = Array.Empty<string>();
        var preferredChangedItems = Array.Empty<int>();

        var localModDatReader = new LocalModDataReaderBuilder().Build();

        var importDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(LocalModData.FileVersion), LocalModDataReader.SUPPORTED_FILE_VERSION },
            { nameof(LocalModData.ImportDate), importDate },
            { nameof(LocalModData.Favorite), true }
        });

        var success = localModDatReader.TryRead(element, out var localModData);

        Assert.IsTrue(success);
        Assert.IsNotNull(localModData);

        Assert.AreEqual(LocalModDataReader.SUPPORTED_FILE_VERSION, localModData.FileVersion);
        Assert.AreEqual(importDate, localModData.ImportDate);
        Assert.AreSame(localTags, localModData.LocalTags);
        Assert.IsTrue(localModData.Favorite);
        Assert.AreSame(preferredChangedItems, localModData.PreferredChangedItems);
    }
}
