using Dalamud.Interface.Windowing;
using ModOrganizer.Windows;
using ModOrganizer.Windows.Managers;

namespace ModOrganizer.Tests.Windows.Managers;

[TestClass]
public class TestWindowManager
{
    [TestMethod]
    public void TestAdd()
    {
        var windowSystem = new WindowSystem();
        
        var windowManager = new WindowManager();
        windowManager.MaybeWindowSystem = windowSystem;

        var aboutWindow = new AboutWindow();
        windowManager.Add(aboutWindow);

        var windows = windowSystem.Windows;
        Assert.HasCount(1, windows);

        Assert.AreSame(aboutWindow, windows[0]);
    }

    [TestMethod]
    public void TestAddWithoutWindowSystem()
    {
        var windowManager = new WindowManager();

        Assert.Throws<NotImplementedException>(() => windowManager.Add(new AboutWindow()));
    }

    [TestMethod]
    public void TestRemove()
    {
        var windowSystem = new WindowSystem();

        var aboutWindow = new AboutWindow();
        windowSystem.AddWindow(aboutWindow);

        var windowManager = new WindowManager();
        windowManager.MaybeWindowSystem = windowSystem;

        windowManager.Remove(aboutWindow);
        Assert.IsEmpty(windowSystem.Windows);
    }

    [TestMethod]
    public void TestRemoveWithoutWindowSystem()
    {
        var windowManager = new WindowManager();

        Assert.Throws<NotImplementedException>(() => windowManager.Remove(new AboutWindow()));
    }
}
