using ModOrganizer.Json.Penumbra.ModMetas;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.Penumbra.ModMetas;

[TestClass]
public class TestModMetaReader
{
    [TestMethod]
    public void TestTryRead()
    {
        var fileVersion = ModMetaReader.SUPPORTED_FILE_VERSION;
        var name = "Name";

        var author = "Author";
        var description = "Description";
        var image = "Image";
        var version = "Version";
        var website = "Website";

        var modTag = "Tag";
        var defaultPreferredItem = 1;
        var requiredFeature = "Feature";

        var reader = new ModMetaReaderBuilder().Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(ModMetaV3.FileVersion), fileVersion },
            { nameof(ModMetaV3.Name), name },

            { nameof(ModMetaV3.Author), author },
            { nameof(ModMetaV3.Description), description },
            { nameof(ModMetaV3.Image), image },
            { nameof(ModMetaV3.Version), version },
            { nameof(ModMetaV3.Website), website },

            { nameof(ModMetaV3.ModTags), new string[] { modTag } },
            { nameof(ModMetaV3.DefaultPreferredItems), new int[] { defaultPreferredItem } },
            { nameof(ModMetaV3.RequiredFeatures), new string[] { requiredFeature } }
        });

        var success = reader.TryRead(element, out var modMeta);

        Assert.IsTrue(success);
        Assert.IsNotNull(modMeta);

        Assert.AreEqual(fileVersion, modMeta.FileVersion);
        Assert.AreEqual(name, modMeta.Name);

        Assert.AreEqual(author, modMeta.Author);
        Assert.AreEqual(description, modMeta.Description);
        Assert.AreEqual(image, modMeta.Image);
        Assert.AreEqual(version, modMeta.Version);
        Assert.AreEqual(website, modMeta.Website);

        Assert.IsNotNull(modMeta.ModTags);
        Assert.HasCount(1, modMeta.ModTags);
        Assert.AreEqual(modTag, modMeta.ModTags[0]);

        Assert.IsNotNull(modMeta.DefaultPreferredItems);
        Assert.HasCount(1, modMeta.DefaultPreferredItems);
        Assert.AreEqual(defaultPreferredItem, modMeta.DefaultPreferredItems[0]);

        Assert.IsNotNull(modMeta.RequiredFeatures);
        Assert.HasCount(1, modMeta.RequiredFeatures);
        Assert.AreEqual(requiredFeature, modMeta.RequiredFeatures[0]);
    }

    [TestMethod]
    public void TestTryReadWithDefaults()
    {
        var fileVersion = ModMetaReader.SUPPORTED_FILE_VERSION;
        var name = "Name";

        var reader = new ModMetaReaderBuilder().Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(ModMetaV3.FileVersion), fileVersion },
            { nameof(ModMetaV3.Name), name }
        });

        var success = reader.TryRead(element, out var modMeta);

        Assert.IsTrue(success);
        Assert.IsNotNull(modMeta);

        Assert.AreEqual(fileVersion, modMeta.FileVersion);
        Assert.AreEqual(name, modMeta.Name);

        Assert.IsNull(modMeta.Author);
        Assert.IsNull(modMeta.Description);
        Assert.IsNull(modMeta.Image);
        Assert.IsNull(modMeta.Version);
        Assert.IsNull(modMeta.Website);

        Assert.IsNull(modMeta.ModTags);
        Assert.IsNull(modMeta.DefaultPreferredItems);
        Assert.IsNull(modMeta.RequiredFeatures);
    }
}
