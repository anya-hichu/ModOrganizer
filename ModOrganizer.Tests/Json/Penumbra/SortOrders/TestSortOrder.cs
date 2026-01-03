using ModOrganizer.Json.Penumbra.SortOrders;

namespace ModOrganizer.Tests.Json.Penumbra.SortOrders;

[TestClass]
public class TestSortOrder
{
    [TestMethod]
    public void TestDefaults()
    {
        var sortOrder = new SortOrder();
        Assert.IsEmpty(sortOrder.Data);
        Assert.IsEmpty(sortOrder.EmptyFolders);
    }
}
