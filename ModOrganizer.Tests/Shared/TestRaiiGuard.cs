using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Systems;

namespace ModOrganizer.Tests.Shared;

[TestClass]
public class TestRaiiGuard
{
    [TestMethod]
    public void Test()
    {
        var acquireObserver = new StubObserver();
        var acquire = StubAction.WithObserver(acquireObserver);

        var releaseObserver = new StubObserver();
        var release = StubAction.WithObserver(releaseObserver);

        Assert.IsEmpty(acquireObserver.GetCalls());
        Assert.IsEmpty(releaseObserver.GetCalls());

        using (new RaiiGuard(acquire, release)) 
        { 
            Assert.HasCount(1, acquireObserver.GetCalls());
            Assert.IsEmpty(releaseObserver.GetCalls());
        }

        Assert.HasCount(1, acquireObserver.GetCalls());
        Assert.HasCount(1, releaseObserver.GetCalls());
    }

    [TestMethod]
    public void TestWithValue()
    {
        var value = 1;

        var acquireObserver = new StubObserver();
        var acquire = StubFunc.WithObserver(acquireObserver, () => value);

        var releaseObserver = new StubObserver();
        var release = StubAction.WithObserver<int>(releaseObserver);

        Assert.IsEmpty(acquireObserver.GetCalls());
        Assert.IsEmpty(releaseObserver.GetCalls());

        using (var resource = new RaiiGuard<int>(acquire, release))
        {
            Assert.HasCount(1, acquireObserver.GetCalls());
            Assert.IsEmpty(releaseObserver.GetCalls());

            Assert.AreEqual(value, resource.Value);
        }

        Assert.HasCount(1, acquireObserver.GetCalls());

        var releaseCalls = releaseObserver.GetCalls();
        Assert.HasCount(1, releaseCalls);

        Assert.AreEqual(value, releaseCalls[0].GetArguments()[0]);
    }
}
