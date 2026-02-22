using ModOrganizer.Json.Penumbra.SortOrders;
using System.Text.Json;

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

    [TestMethod]
    public void TestToString()
    {
        var sortOrder = new SortOrder();

        Assert.AreEqual("SortOrder", sortOrder.ToString());
    }
}
