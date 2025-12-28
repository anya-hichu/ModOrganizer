using Dalamud.Plugin.Fakes;

namespace ModOrganizer.Tests.Shared.PluginInterfaces;

public interface IStubbablePluginInterface
{
    StubIDalamudPluginInterface PluginInterfaceStub { get; }
}
