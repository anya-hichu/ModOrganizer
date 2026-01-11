using Dalamud.Bindings.ImGui;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Tests.Dalamuds.Bindings.ImGuis;

namespace ModOrganizer.Tests.Windows.Abouts;

[TestClass]
public class TestAboutWindow
{
    [TestMethod]
    [DoNotParallelize]
    public void TestDraw()
    {
        var observer = new StubObserver();

        using var aboutWindowResource = new AboutWindowResourceBuilder()
            .WithImGuiDefaults()
            .WithImGuiObserver(observer)
            .Build();

        aboutWindowResource.Value.Draw();

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        var call = calls[0];
        Assert.AreEqual(nameof(ImGui.Text), call.StubbedMethod.Name);

        Assert.AreEqual("About", call.GetArguments()[0].ToString());
    }
}
