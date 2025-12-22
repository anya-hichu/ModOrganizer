using Dalamud.Plugin.Fakes;

namespace ModOrganizer.Tests.Stubbables;

public interface IStubbablePluginInterface
{
    StubIDalamudPluginInterface PluginInterfaceStub { get; }
}
