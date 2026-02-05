using Dalamud.Interface.Windowing;
using ModOrganizer.Windows;
using ModOrganizer.Windows.Togglers;

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

        var windowToggle = new WindowToggler();
        windowToggle.MaybeWindowSystem = windowSystem;

        windowToggle.Toggle<AboutWindow>();

        Assert.IsTrue(aboutWindow.IsOpen);

        windowToggle.Toggle<AboutWindow>();

        Assert.IsFalse(aboutWindow.IsOpen);
    }


    [TestMethod]
    public void TestToggleWithoutWindowSystem()
    {
        var windowToggle = new WindowToggler();

        Assert.Throws<NotImplementedException>(windowToggle.Toggle<AboutWindow>);
    }
}
