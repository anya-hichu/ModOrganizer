using ModOrganizer.Json.Penumbra.Groups;
using ModOrganizer.Tests.Json.Penumbra.Groups.Bases;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.Penumbra.Groups.Imcs;

[TestClass]
public class TestGroupImcReader
{
    [TestMethod]
    public void TestTryReadWithDefaults()
    {
        var name = "Group Name";
        var type = GroupImcReader.TYPE;

        var baseGroup = new Group()
        {
            Name = name,
            Type = type
        };

        var reader = new GroupImcReaderBuilder()
            .WithGroupBaseReaderTryRead(baseGroup)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>());

        var success = reader.TryRead(element, out var group);

        Assert.IsTrue(success);
        Assert.IsNotNull(group);

        Assert.AreEqual(name, group.Name);
        Assert.AreEqual(type, group.Type);

        var groupImc = group as GroupImc;
        Assert.IsNotNull(groupImc);

        Assert.IsNull(groupImc.AllVariants);
        Assert.IsNull(groupImc.AllAttributes);

        Assert.IsNull(groupImc.DefaultEntry);
        Assert.IsNull(groupImc.Identifier);

        Assert.IsNotNull(groupImc.Options);
        Assert.IsEmpty(groupImc.Options);
    }
}
