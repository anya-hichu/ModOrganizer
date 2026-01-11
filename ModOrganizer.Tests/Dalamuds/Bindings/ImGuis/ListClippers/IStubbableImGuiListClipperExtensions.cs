using Microsoft.QualityTools.Testing.Fakes.Stubs;

namespace ModOrganizer.Tests.Dalamuds.Bindings.ImGuis.ListClippers;

public static class IStubbableImGuiListClipperExtensions
{
    public static T WithImGuiListClipperStub<T>(this T stubbable) where T : IStubbableImGuiListClipper
    {
        stubbable.ImGuiStub.ImGuiListClipper = () => new()
        {
            MaybeImplementation = stubbable.ImGuiListClipperPtrStub
        };

        return stubbable;
    }
    public static T WithImGuiListClipperDefaults<T>(this T stubbable) where T : IStubbableImGuiListClipper
    {
        stubbable.ImGuiListClipperPtrStub.BehaveAsDefaultValue();

        return stubbable;
    }

    public static T WithImGuiListClipperObserver<T>(this T stubbable, IStubObserver observer) where T : IStubbableImGuiListClipper
    {
        stubbable.ImGuiListClipperPtrStub.InstanceObserver = observer;

        return stubbable;
    }
}
