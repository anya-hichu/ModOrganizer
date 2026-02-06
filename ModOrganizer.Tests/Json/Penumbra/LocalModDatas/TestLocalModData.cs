using ModOrganizer.Json.Penumbra.LocalModDatas;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.Penumbra.LocalModDatas;

[TestClass]
public class TestLocalModData
{
    [TestMethod]
    public void TestTryRead()
    {
        var fileVersion = 3u;
        var importDate = 2;
        var localTag = "Local Tag";
        var preferredChangedItem = 1;

        var reader = new LocalModDataReaderBuilder().Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(LocalModDataV3.FileVersion), fileVersion },
            { nameof(LocalModDataV3.ImportDate), importDate },
            { nameof(LocalModDataV3.LocalTags), new string[] { localTag } },
            { nameof(LocalModDataV3.Favorite), true },
            { nameof(LocalModDataV3.PreferredChangedItems), new int[] { preferredChangedItem } }
        });

        var success = reader.TryRead(element, out var localModData);

        Assert.IsTrue(success);
        Assert.IsNotNull(localModData);

        Assert.AreEqual(fileVersion, localModData.FileVersion);
        Assert.AreEqual(importDate, localModData.ImportDate);

        Assert.IsNotNull(localModData.LocalTags);
        Assert.HasCount(1, localModData.LocalTags);
        Assert.AreEqual(localTag, localModData.LocalTags[0]);

        Assert.IsTrue(localModData.Favorite);

        Assert.IsNotNull(localModData.PreferredChangedItems);
        Assert.HasCount(1, localModData.PreferredChangedItems);
        Assert.AreEqual(preferredChangedItem, localModData.PreferredChangedItems[0]);
    }
}
