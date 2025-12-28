using Dalamud.Plugin.Services.Fakes;

namespace ModOrganizer.Tests.Shared.CommandManager;

public interface IStubbableCommandManager
{
    StubICommandManager CommandManagerStub { get; }
}
