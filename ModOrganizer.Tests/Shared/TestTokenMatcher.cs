using ModOrganizer.Shared;

namespace ModOrganizer.Tests.Shared;

[TestClass]
public class TestTokenMatcher
{
    [TestMethod]
    [DataRow("s")]
    [DataRow("should")]
    [DataRow("should match")]
    [DataRow(" match THIS should ")]
    public void TestMatches(string filter)
    {
        var success = TokenMatcher.Matches(filter, "this should match");

        Assert.IsTrue(success);
    }

    [TestMethod]
    [DataRow("'")]
    [DataRow("shouldn't")]
    [DataRow("shouldn't match")]
    [DataRow(" match THIS should't ")]
    public void TestMatchesWithoutSuccess(string filter)
    {
        var success = TokenMatcher.Matches(filter, "this should not match");

        Assert.IsFalse(success);
    }

    [TestMethod]
    public void TestMatchesWithEmptyFilter()
    {
        var success = TokenMatcher.Matches(string.Empty, "anything");

        Assert.IsTrue(success);
    }

    [TestMethod]
    public void TestMatchesWithNullText()
    {
        var success = TokenMatcher.Matches("Filter", null);

        Assert.IsFalse(success);
    }

    [TestMethod]
    [DataRow("s")]
    [DataRow("should")]
    [DataRow("should match")]
    [DataRow(" match THIS should ")]
    public void TestMatchesMany(string filter)
    {
        var success = TokenMatcher.MatchesMany(filter, ["this", "should", "match", null]);

        Assert.IsTrue(success);
    }

    [TestMethod]
    [DataRow("'")]
    [DataRow("shouldn't")]
    [DataRow("shouldn't match")]
    [DataRow(" match THIS should't ")]
    public void TestMatchesManyWithoutSuccess(string filter)
    {
        var success = TokenMatcher.MatchesMany(filter, ["this", "should not", "match", null]);

        Assert.IsFalse(success);
    }

    [TestMethod]
    public void TestMatchesManyWithEmptyFilter()
    {
        var success = TokenMatcher.MatchesMany(string.Empty, []);

        Assert.IsTrue(success);
    }
}
