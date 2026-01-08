using ModOrganizer.Shared;

namespace ModOrganizer.Tests.Shared;

[TestClass]
public class TestTokenMatcher
{
    [TestMethod]
    public void TestMatchesWithNullText()
    {
        var success = TokenMatcher.Matches("Filter", null);

        Assert.IsFalse(success);
    }
}
