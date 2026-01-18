using ModOrganizer.Json.Penumbra.Groups;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.Penumbra.Groups.Bases;

[TestClass]
public class TestGroupBaseReader
{
    [TestMethod]
    public void TestTryRead()
    {
        var groupBaseReader = new GroupBaseReaderBuilder().Build();

        var version = 0u;
        var name = "Name";
        var description = "Description";
        var image = "Image";
        var page = 1;
        var priority = 2;
        var type = "Type";
        var defaultSettings = 3;

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(Group.Version), version },
            { nameof(Group.Name), name },
            { nameof(Group.Description), description },
            { nameof(Group.Image), image },
            { nameof(Group.Page), page },
            { nameof(Group.Priority), priority },
            { nameof(Group.Type), type },
            { nameof(Group.DefaultSettings), defaultSettings }
        });

        var success = groupBaseReader.TryRead(element, out var group);

        Assert.IsTrue(success);
        Assert.IsNotNull(group);

        Assert.AreEqual(version, group.Version);
        Assert.AreEqual(name, group.Name);
        Assert.AreEqual(description, group.Description);
        Assert.AreEqual(image, group.Image);
        Assert.AreEqual(page, group.Page);
        Assert.AreEqual(priority, group.Priority);
        Assert.AreEqual(type, group.Type);
        Assert.AreEqual(defaultSettings, group.DefaultSettings);
    }
}
