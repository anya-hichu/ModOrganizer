using ModOrganizer.Mods.Fakes;
using ModOrganizer.Shared;

namespace ModOrganizer.Tests.Shared;

[TestClass]
public class TestMemberRenamer
{
    [TestMethod]
    public void TestRename()
    {
        var instance = new { __PropertyName__ = true };

        var memberInfo = instance.GetType().GetMember(nameof(instance.__PropertyName__))[0];

        var renamed = MemberRenamer.Rename(memberInfo);

        Assert.AreEqual("property_name", renamed);
    }
}
