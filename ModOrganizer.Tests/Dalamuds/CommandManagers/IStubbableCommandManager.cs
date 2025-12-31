using Dalamud.Plugin.Services.Fakes;

namespace ModOrganizer.Tests.Dalamuds.CommandManagers;

public interface IStubbableCommandManager
{
    StubICommandManager CommandManagerStub { get; }
}
