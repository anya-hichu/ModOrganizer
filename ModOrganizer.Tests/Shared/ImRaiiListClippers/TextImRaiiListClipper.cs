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

        var imRaiiListClipperResource = new ImRaiiListClipperResourceBuilder()
            .WithImGuiListClipperDefaults()
            .WithImGuiListClipperObserver(observer)
            .WithItemsCount(count)
            .WithItemsHeight(height)
            .Build();

        var beforeCalls = observer.GetCalls();
        Assert.HasCount(1, beforeCalls);

        var beforeCall = beforeCalls[0];
        Assert.AreEqual(nameof(ImGuiListClipperPtr.Begin), beforeCall.StubbedMethod.Name);

        var beforeCallArguments = beforeCall.GetArguments();
        Assert.AreEqual(count, beforeCallArguments[0]);
        Assert.AreEqual(height, beforeCallArguments[1]);

        imRaiiListClipperResource.Dispose();

        var afterCalls = observer.GetCalls();
        Assert.HasCount(3, afterCalls);

        Assert.AreEqual(nameof(ImGuiListClipperPtr.End), afterCalls[1].StubbedMethod.Name);
        Assert.AreEqual(nameof(ImGuiListClipperPtr.Destroy), afterCalls[2].StubbedMethod.Name);
    }
}
