using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Tests.Shared.ImRaiiListClippers;

namespace ModOrganizer.Tests.Dalamuds.Bindings.ImGuis.ListClippers;

public static class IStubbableImGuiListClipperExtensions
{
    public static T WithImGuiListClipperItemsCount<T>(this T stubbable, int count) where T : IStubbableImGuiListClipper
    {
        stubbable.ImGuiListClipperItemsCount = count;

        return stubbable;
    }

    public static T WithImGuiListClipperItemsHeight<T>(this T stubbable, float height) where T : IStubbableImGuiListClipper
    {
        stubbable.ImGuiListClipperItemsHeight = height;

        return stubbable;
    }

    public static T WithImGuiListClipperDefaults<T>(this T stubbable) where T : IStubbableImGuiListClipper
    {
        stubbable.ImGuiListClipperStub.BehaveAsDefaultValue();

        return stubbable;
    }

    public static T WithImGuiListClipperObserver<T>(this T stubbable, IStubObserver observer) where T : IStubbableImGuiListClipper
    {
        stubbable.ImGuiListClipperStub.InstanceObserver = observer;

        return stubbable;
    }
}
