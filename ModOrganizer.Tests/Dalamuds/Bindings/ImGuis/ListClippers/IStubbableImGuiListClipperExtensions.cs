using Microsoft.QualityTools.Testing.Fakes.Stubs;

namespace ModOrganizer.Tests.Dalamuds.Bindings.ImGuis.ListClippers;

public static class IStubbableImGuiListClipperExtensions
{
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
