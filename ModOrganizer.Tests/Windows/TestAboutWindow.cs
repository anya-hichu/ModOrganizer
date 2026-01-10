using Dalamud.Bindings.ImGui;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Tests.Dalamuds.Bindings.ImGuis;
using ModOrganizer.Windows;

namespace ModOrganizer.Tests.Windows;

[TestClass]
public class TestAboutWindow
{
    [TestMethod]
    [DoNotParallelize]
    public void TestDraw()
    {
        var observer = new StubObserver();

        var aboutWindow = new AboutWindow();

        var stub = new ImGuiStubBuilder()
            .WithDefaults()
            .WithObserver(observer)
            .Build();

        aboutWindow.Draw();

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        var call = calls[0];
        Assert.AreEqual(nameof(ImGui.Text), call.StubbedMethod.Name);

        Assert.AreEqual("About", call.GetArguments()[0].ToString());
    }
}
