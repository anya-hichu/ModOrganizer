using Dalamud.Plugin.Services.Fakes;

namespace ModOrganizer.Tests.Dalamuds.PluginLogs;

public interface IStubbablePluginLog
{
    StubIPluginLog PluginLogStub { get;}
}
