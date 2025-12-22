using Dalamud.Plugin.Services.Fakes;

namespace ModOrganizer.Tests.Stubbables;

public interface IStubbableNotificationManager
{
    StubINotificationManager NotificationManagerStub { get; }
}
