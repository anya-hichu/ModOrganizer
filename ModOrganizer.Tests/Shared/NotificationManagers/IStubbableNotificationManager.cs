using Dalamud.Plugin.Services.Fakes;

namespace ModOrganizer.Tests.Shared.NotificationManager;

public interface IStubbableNotificationManager
{
    StubINotificationManager NotificationManagerStub { get; }
}
