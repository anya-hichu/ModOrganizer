using Dalamud.Plugin.Services.Fakes;

namespace ModOrganizer.Tests.Stubbables;

public interface IStubbableCommandManager
{
    StubICommandManager CommandManagerStub { get; }
}
