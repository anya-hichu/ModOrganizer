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

        var success = multiMatcher.Matches((VirtualFolder)new());

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
        
        var success = multiMatcher.Matches((VirtualFolder)new());

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

        var success = multiMatcher.Matches((VirtualFile)new());

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

        var success = multiMatcher.Matches((VirtualFile)new());

        Assert.IsFalse(success);
    }
}
