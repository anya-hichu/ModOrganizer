using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Dalamuds.PluginLogs;

using Assert = ModOrganizer.Json.Readers.Asserts.Assert;

namespace ModOrganizer.Tests.Json.Readers.Asserts;

public class AssertBuilder : IBuilder<Assert>, IStubbablePluginLog
{
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public Assert Build() => new(PluginLogStub);
}
