using Dalamud.Plugin.Services.Fakes;

namespace ModOrganizer.Tests.Shared.PluginLogs;

public interface IStubbablePluginLog
{
    StubIPluginLog PluginLogStub { get;}
}
