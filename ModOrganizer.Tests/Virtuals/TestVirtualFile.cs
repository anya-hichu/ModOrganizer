using ModOrganizer.Virtuals;

namespace ModOrganizer.Tests.Virtuals;

[TestClass]
public class TestVirtualFile
{
    [TestMethod]
    public void TestCompareTo()
    {
        var left = new VirtualFile() { Name = "Name Left" };
        var right = new VirtualFile() { Name = "Name Right" };
        
        var value = left.CompareTo(right);

        Assert.IsLessThan(0, value);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("Path")]
    public void TestEquals(string path)
    {
        var left = new VirtualFile()
        {
            Name = "Name Left",
            Directory = "Directory Left",
            Path = path
        };

        var right = new VirtualFile()
        {
            Name = "Name Right",
            Directory = "Directory Right",
            Path = path
        };

        var success = left.Equals(right);

        Assert.IsTrue(success);
    }

    [TestMethod]
    [DataRow("", " ")]
    [DataRow("Path Left", "Path Right")]
    public void TestEqualsWithoutSuccess(string leftPath, string rightPath)
    {
        var left = new VirtualFile() { Path = leftPath };
        var right = new VirtualFile() { Path = rightPath };

        var success = left.Equals(right);

        Assert.IsFalse(success);
    }
}
