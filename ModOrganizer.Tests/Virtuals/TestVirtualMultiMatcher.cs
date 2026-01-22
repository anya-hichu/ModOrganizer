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
            MatchesVirtualFolder = _ => true
        };

        var multiMatcher = new VirtualMultiMatcher([matcher, matcher]);

        var success = multiMatcher.Matches(new VirtualFolder());

        Assert.IsTrue(success);
    }

    [TestMethod]
    public void TestMatchesFolderWithoutSuccess()
    {
        var firstMatcher = new StubIVirtualMatcher()
        {
            InstanceBehavior = StubBehaviors.NotImplemented,
            MatchesVirtualFolder = _ => true 
        };

        var secondMatcher = new StubIVirtualMatcher()
        {
            InstanceBehavior = StubBehaviors.NotImplemented,
            MatchesVirtualFolder = _ => false
        };

        var multiMatcher = new VirtualMultiMatcher([firstMatcher, secondMatcher]);
        
        var success = multiMatcher.Matches(new VirtualFolder());

        Assert.IsFalse(success);
    }

    [TestMethod]
    public void TestMatchesFile()
    {
        var matcher = new StubIVirtualMatcher()
        {
            InstanceBehavior = StubBehaviors.NotImplemented,
        };

        var multiMatcher = new VirtualMultiMatcher([matcher, matcher]);

        var success = multiMatcher.Matches(new VirtualFile());

        Assert.IsTrue(success);
    }

    [TestMethod]
    public void TestMatchesFileWithoutSuccess()
    {
        var firstMatcher = new StubIVirtualMatcher()
        {
            InstanceBehavior = StubBehaviors.NotImplemented,
            MatchesVirtualFile = _ => true
        };

        var secondMatcher = new StubIVirtualMatcher()
        {
            InstanceBehavior = StubBehaviors.NotImplemented,
            MatchesVirtualFile = _ => false
        };

        var multiMatcher = new VirtualMultiMatcher([firstMatcher, secondMatcher]);

        var success = multiMatcher.Matches(new VirtualFile());

        Assert.IsFalse(success);
    }
}
