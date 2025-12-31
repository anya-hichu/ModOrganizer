using Dalamud.Plugin.Fakes;

namespace ModOrganizer.Tests.Dalamuds.PluginInterfaces;

public interface IStubbablePluginInterface
{
    StubIDalamudPluginInterface PluginInterfaceStub { get; }
}
