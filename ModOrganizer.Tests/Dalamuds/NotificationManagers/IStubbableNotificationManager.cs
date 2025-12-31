using Dalamud.Plugin.Services.Fakes;

namespace ModOrganizer.Tests.Dalamuds.NotificationManagers;

public interface IStubbableNotificationManager
{
    StubINotificationManager NotificationManagerStub { get; }
}
