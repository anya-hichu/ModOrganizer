using ModOrganizer.Virtuals;

namespace ModOrganizer.Tests.Virtuals;

[TestClass]
public class TestVirtualFolder
{
    [TestMethod]
    [DataRow("", 2)]
    [DataRow("2", 2)]
    [DataRow("4", 1)]
    [DataRow("7", 1)]
    public void TestTrySearch(string filter, int expectedMatchingFiles)
    {
        var folder = new VirtualFolder()
        {
            Name = "1",
            Path = "2",
            Files = [
                new()
                {
                    Name = "3",
                    Directory = "4",
                    Path = "2/4"
                }
            ],
            Folders = [
                new()
                {
                    Name = "5",
                    Path = "2/5",
                    Files = [
                        new()
                        {
                            Name = "6",
                            Directory = "7",
                            Path = "2/3/7"
                        }
                    ]
                }
            ]
        };

        var success = folder.TrySearch(filter, out var filteredFolder);

        Assert.IsTrue(success);
        Assert.IsNotNull(filteredFolder);

        Assert.AreEqual(expectedMatchingFiles, filteredFolder.GetNestedFiles().Count());
    }

    [TestMethod]
    [DataRow("-")]
    [DataRow("1")]
    [DataRow("5")]
    [DataRow("8")]
    public void TestTrySearchWithoutSuccess(string filter)
    {
        var folder = new VirtualFolder()
        {
            Name = "1",
            Path = "2",
            Files = [
                new()
                {
                    Name = "3",
                    Directory = "4",
                    Path = "2/4"
                }
            ],
            Folders = [
                new()
                {
                    Name = "5",
                    Path = "2/5",
                    Files = [
                        new()
                        {
                            Name = "6",
                            Directory = "7",
                            Path = "2/3/7"
                        }
                    ]
                }
            ]
        };

        var success = folder.TrySearch(filter, out var filteredFolder);

        Assert.IsFalse(success);
        Assert.IsNull(filteredFolder);
    }

    [TestMethod]
    public void TestIsEmpty()
    {
        var folder = new VirtualFolder() { Folders = [new()] };

        var success = folder.IsEmpty();

        Assert.IsTrue(success);
    }


    [TestMethod]
    public void TestIsEmptyWithoutSuccess()
    {
        var folder = new VirtualFolder() { Folders = [new() { Files = [new()] }] };

        var success = folder.IsEmpty();

        Assert.IsFalse(success
            );
    }


    [TestMethod]
    public void TestGetNestedFiles()
    {
        var nestedFile = new VirtualFile();

        var folder = new VirtualFolder() { Folders = [new() { Files = [nestedFile] }] };

        var nestedFiles = folder.GetNestedFiles();

        Assert.HasCount(1, nestedFiles);
        Assert.AreSame(nestedFile, nestedFiles.First());
    }

    [TestMethod]
    public void TestCompareTo()
    {
        var left = new VirtualFolder() { Name = "Name Left" };
        var right = new VirtualFolder() { Name = "Name Right" };

        var value = left.CompareTo(right);

        Assert.IsLessThan(0, value);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("Path")]
    public void TestEquals(string path)
    {
        var left = new VirtualFolder()
        {
            Name = "Name Left",
            Path = path
        };

        var right = new VirtualFolder()
        {
            Name = "Name Right",
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
        var left = new VirtualFolder() { Path = leftPath };
        var right = new VirtualFolder() { Path = rightPath };

        var success = left.Equals(right);

        Assert.IsFalse(success);
    }
}
