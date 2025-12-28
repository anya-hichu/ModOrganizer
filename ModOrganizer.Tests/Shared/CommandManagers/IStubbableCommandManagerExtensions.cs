namespace ModOrganizer.Tests.Shared.CommandManager;

public static class IStubbableCommandManagerExtensions
{
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
}
