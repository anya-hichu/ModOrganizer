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

        var itemsCount = 1;
        var itemsHeight = 2f;

        var imRaiiListClipperResource = new ImRaiiListClipperResourceBuilder()
            .WithImGuiListClipperDefaults()
            .WithImGuiListClipperObserver(observer)
            .WithItemsCount(itemsCount)
            .WithItemsHeight(itemsHeight)
            .Build();

        var beforeCalls = observer.GetCalls();
        Assert.HasCount(1, beforeCalls);

        var beforeCall = beforeCalls[0];
        Assert.AreEqual(nameof(ImGuiListClipperPtr.Begin), beforeCall.StubbedMethod.Name);

        var beforeCallArguments = beforeCall.GetArguments();
        Assert.AreEqual(itemsCount, beforeCallArguments[0]);
        Assert.AreEqual(itemsHeight, beforeCallArguments[1]);

        imRaiiListClipperResource.Dispose();

        var afterCalls = observer.GetCalls();
        Assert.HasCount(3, afterCalls);

        Assert.AreEqual(nameof(ImGuiListClipperPtr.End), afterCalls[1].StubbedMethod.Name);
        Assert.AreEqual(nameof(ImGuiListClipperPtr.Destroy), afterCalls[2].StubbedMethod.Name);
    }
}
