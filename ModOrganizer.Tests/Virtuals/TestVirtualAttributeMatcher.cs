using ModOrganizer.Virtuals;

namespace ModOrganizer.Tests.Virtuals;

[TestClass]
public class TestVirtualAttributeMatcher
{
    [TestMethod]
    [DataRow("")]
    [DataRow("this")]
    [DataRow("should match")]
    [DataRow("THIS match should")]
    public void TestMatches(string filter)
    {
        var matcher = new VirtualAttributesMatcher(filter);

        var file = new VirtualFile()
        {
            Name = "this",
            Directory = "should",
            Path = "match"
        };

        var success = matcher.Matches(file);

        Assert.IsTrue(success);
    }

    [TestMethod]
    [DataRow("'")]
    [DataRow("but this")]
    [DataRow("should't match")]
    [DataRow("THIS match should't")]
    public void TestMatchesWithoutSuccess(string filter)
    {
        var matcher = new VirtualAttributesMatcher(filter);

        var file = new VirtualFile()
        {
            Name = "this",
            Directory = "should not",
            Path = "match"
        };

        var success = matcher.Matches(file);

        Assert.IsFalse(success);
    }
}
