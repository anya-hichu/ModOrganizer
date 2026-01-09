using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Virtuals;
using ModOrganizer.Virtuals.Fakes;

namespace ModOrganizer.Tests.Virtuals;

[TestClass]
public class TestVirtualMultiMatcher
{
    [TestMethod]
    public void TestMatchesFolder()
    {
        var matcher = new StubIVirtualMatcher()
        {
            InstanceBehavior = StubBehaviors.NotImplemented,
            MatchesFolderVirtualFolder = _ => true, 
        };

        var multiMatcher = new VirtualMultiMatcher([matcher, matcher]);
        var folder = VirtualFolder.BuildRoot();

        var success = multiMatcher.MatchesFolder(folder);

        Assert.IsTrue(success);
    }

    [TestMethod]
    public void TestMatchesFolderWithoutSuccess()
    {
        var firstMatcher = new StubIVirtualMatcher()
        {
            InstanceBehavior = StubBehaviors.NotImplemented,
            MatchesFolderVirtualFolder = _ => true 
        };

        var secondMatcher = new StubIVirtualMatcher()
        {
            InstanceBehavior = StubBehaviors.NotImplemented,
            MatchesFolderVirtualFolder = _ => false
        };

        var multiMatcher = new VirtualMultiMatcher([firstMatcher, secondMatcher]);
        var folder = VirtualFolder.BuildRoot();

        var success = multiMatcher.MatchesFolder(folder);

        Assert.IsFalse(success);
    }

    [TestMethod]
    public void TestMatchesFile()
    {
        var matcher = new StubIVirtualMatcher()
        {
            InstanceBehavior = StubBehaviors.NotImplemented,
            MatchesFileVirtualFile = _ => true,
        };

        var multiMatcher = new VirtualMultiMatcher([matcher, matcher]);

        var file = new VirtualFile()
        {
            Name = string.Empty,
            Directory = string.Empty,
            Path = string.Empty
        };

        var success = multiMatcher.MatchesFile(file);

        Assert.IsTrue(success);
    }

    [TestMethod]
    public void TestMatchesFileWithoutSuccess()
    {
        var firstMatcher = new StubIVirtualMatcher()
        {
            InstanceBehavior = StubBehaviors.NotImplemented,
            MatchesFileVirtualFile = _ => true,
        };

        var secondMatcher = new StubIVirtualMatcher()
        {
            InstanceBehavior = StubBehaviors.NotImplemented,
            MatchesFileVirtualFile = _ => false,
        };

        var multiMatcher = new VirtualMultiMatcher([firstMatcher, secondMatcher]);

        var file = new VirtualFile()
        {
            Name = string.Empty,
            Directory = string.Empty,
            Path = string.Empty
        };

        var success = multiMatcher.MatchesFile(file);

        Assert.IsFalse(success);
    }
}
