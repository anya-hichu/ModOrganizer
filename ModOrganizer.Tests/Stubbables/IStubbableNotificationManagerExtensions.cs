using Microsoft.QualityTools.Testing.Fakes.Stubs;

namespace ModOrganizer.Tests.Stubbables;

public static class IStubbableNotificationManagerExtensions
{
    public static T WithNotificationManagerDefaults<T>(this T stubbable) where T : IStubbableNotificationManager
    {
        stubbable.NotificationManagerStub.BehaveAsDefaultValue();

        return stubbable;
    }

    public static T WithNotificationManagerObserver<T>(this T stubbable, IStubObserver observer) where T : IStubbableNotificationManager
    {
        stubbable.NotificationManagerStub.InstanceObserver = observer;

        return stubbable;
    }
}
