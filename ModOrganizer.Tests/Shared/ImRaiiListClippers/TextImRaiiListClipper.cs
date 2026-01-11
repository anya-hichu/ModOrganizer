using Dalamud.Bindings.ImGui;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Tests.Dalamuds.Bindings.ImGuis.ListClippers;

namespace ModOrganizer.Tests.Shared.ImRaiiListClippers;

[TestClass]
public class TextImRaiiListClipper
{
    [TestMethod]
    [DoNotParallelize]
    public void Test()
    {
        var observer = new StubObserver();

        var count = 1;
        var height = 2f;

        var clipperResource = new ImRaiiListClipperResourceBuilder()
            .WithImGuiListClipperStub()
            .WithImGuiListClipperDefaults()
            .WithImGuiListClipperObserver(observer)
            .WithImRaiiListClipperItemsCount(count)
            .WithImRaiiListClipperItemsHeight(height)
            .Build();

        var beforeCalls = observer.GetCalls();
        Assert.HasCount(1, beforeCalls);

        var beforeCall = beforeCalls[0];
        Assert.AreEqual(nameof(IImGuiListClipperPtr.Begin), beforeCall.StubbedMethod.Name);

        var beforeCallArguments = beforeCall.GetArguments();
        Assert.AreEqual(count, beforeCallArguments[0]);
        Assert.AreEqual(height, beforeCallArguments[1]);

        clipperResource.Dispose();

        var afterCalls = observer.GetCalls();
        Assert.HasCount(3, afterCalls);

        Assert.AreEqual(nameof(IImGuiListClipperPtr.End), afterCalls[1].StubbedMethod.Name);
        Assert.AreEqual(nameof(IImGuiListClipperPtr.Destroy), afterCalls[2].StubbedMethod.Name);
    }
}
