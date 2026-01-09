using Microsoft.QualityTools.Testing.Fakes.Stubs;

namespace ModOrganizer.Tests.Windows.Togglers;

public static class IStubbableWindowTogglerExtensions
{
    public static T WithStubbableWindowTogglerDefaults<T>(this T stubbable) where T : IStubbableWindowToggler
    {
        stubbable.WindowTogglerStub.BehaveAsDefaultValue();

        return stubbable;
    }

    public static T WithStubbableWindowTogglerObserver<T>(this T stubbable, IStubObserver observer) where T : IStubbableWindowToggler
    {
        stubbable.WindowTogglerStub.InstanceObserver = observer;

        return stubbable;
    }
}
