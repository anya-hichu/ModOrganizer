using ModOrganizer.Json.Penumbra.Groups;
using ModOrganizer.Tests.Json.Penumbra.Groups.Bases;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.Penumbra.Groups.Combinings;

[TestClass]
public class TestGroupCombiningReader
{
    [TestMethod]
    public void TestTryRead()
    {
        var name = "Group Name";
        var type = GroupCombiningReader.TYPE;

        var group = new Group()
        {
            Name = name,
            Type = type
        };

        var groupCombiningReader = new GroupCombiningReaderBuilder()
            .WithGroupBaseReaderTryRead(group)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>());

        var success = groupCombiningReader.TryRead(element, out var actualGroup);

        Assert.IsTrue(success);

        if (actualGroup is not GroupCombining groupCombining) 
        {
            Assert.Fail("Group is not GroupCombining");
            return;
        } 

        Assert.AreEqual(name, groupCombining.Name);
        Assert.AreEqual(type, groupCombining.Type);
        Assert.IsNull(groupCombining.DefaultSettings);

        Assert.IsNotNull(groupCombining.Options);
        Assert.IsEmpty(groupCombining.Options);

        Assert.IsNotNull(groupCombining.Containers);
        Assert.IsEmpty(groupCombining.Containers);

    }

    [TestMethod]
    public void TestTryReadWithInvalidBaseGroup()
    {

    }
}
