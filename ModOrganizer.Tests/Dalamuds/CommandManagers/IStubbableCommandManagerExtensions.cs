using Microsoft.QualityTools.Testing.Fakes.Stubs;

namespace ModOrganizer.Tests.Dalamuds.CommandManagers;

public static class IStubbableCommandManagerExtensions
{
    public static T WithCommandManagerObserver<T>(this T stubbable, IStubObserver observer) where T : IStubbableCommandManager
    {
        stubbable.CommandManagerStub.InstanceObserver = observer;

        return stubbable;
    }

    public static T WithCommandManagerAddHandler<T>(this T stubbable, bool stubValue) where T : IStubbableCommandManager
    {
        stubbable.CommandManagerStub.AddHandlerStringCommandInfo = (command, info) => stubValue;

        return stubbable;
    }

    public static T WithCommandManagerRemoveHandler<T>(this T stubbable, bool stubValue) where T : IStubbableCommandManager
    {
        stubbable.CommandManagerStub.RemoveHandlerString = command => stubValue;

        return stubbable;
    }

    public static T WithCommandManagerProcessCommand<T>(this T stubbable, bool stubValue) where T : IStubbableCommandManager
    {
        stubbable.CommandManagerStub.ProcessCommandString = command => stubValue;

        return stubbable;
    }
}
