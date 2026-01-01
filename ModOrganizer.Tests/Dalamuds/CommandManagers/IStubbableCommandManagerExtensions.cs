using Microsoft.QualityTools.Testing.Fakes.Stubs;

namespace ModOrganizer.Tests.Dalamuds.CommandManagers;

public static class IStubbableCommandManagerExtensions
{
    public static T WithCommandManagerObserver<T>(this T stubbable, IStubObserver observer) where T : IStubbableCommandManager
    {
        stubbable.CommandManagerStub.InstanceObserver = observer;

        return stubbable;
    }

    public static T WithCommandManagerAddHandler<T>(this T stubbable, bool value) where T : IStubbableCommandManager
    {
        stubbable.CommandManagerStub.AddHandlerStringCommandInfo = (command, info) => value;

        return stubbable;
    }

    public static T WithCommandManagerRemoveHandler<T>(this T stubbable, bool value) where T : IStubbableCommandManager
    {
        stubbable.CommandManagerStub.RemoveHandlerString = command => value;

        return stubbable;
    }

    public static T WithCommandManagerProcessCommand<T>(this T stubbable, bool value) where T : IStubbableCommandManager
    {
        stubbable.CommandManagerStub.ProcessCommandString = command => value;

        return stubbable;
    }
}
