using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Windows;

namespace ModOrganizer.Tests.Windows.Togglers;

[TestClass]
public class TestWindowToggle
{
    [TestMethod]
    public void TestToggle()
    {
        var windowSystem = new WindowSystem();
        var aboutWindow = new AboutWindow();
        windowSystem.AddWindow(aboutWindow);

        var windowToggle = new WindowToggleBuilder().Build();
        windowToggle.MaybeWindowSystem = windowSystem;

        windowToggle.Toggle<AboutWindow>();

        Assert.IsTrue(aboutWindow.IsOpen);

        windowToggle.Toggle<AboutWindow>();

        Assert.IsFalse(aboutWindow.IsOpen);
    }


    [TestMethod]
    public void TestToggleWithoutWindowSystem()
    {
        var observer = new StubObserver();

        var windowToggle = new WindowToggleBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        windowToggle.Toggle<AboutWindow>();

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Error), 
            actualMessage => Assert.AreEqual("Failed to toggle [AboutWindow] because window system is not defined", actualMessage));
    }
}
