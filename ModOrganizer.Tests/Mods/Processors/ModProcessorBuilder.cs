using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Backups.Fakes;
using ModOrganizer.Configs.Fakes;
using ModOrganizer.Mods;
using ModOrganizer.Mods.Fakes;
using ModOrganizer.Rules.Fakes;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Backups;
using ModOrganizer.Tests.Configs;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Mods.Interops;
using ModOrganizer.Tests.Rules.Evaluators;

namespace ModOrganizer.Tests.Mods.Processors;

public class ModProcessorBuilder : IBuilder<ModProcessor>, IStubbableBackupManager, IStubbableConfig, IStubbableModInterop, IStubbablePluginLog, IStubbableRuleEvaluator
{
    public StubIBackupManager BackupManagerStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIConfig ConfigStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIModInterop ModInteropStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIRuleEvaluator RuleEvaluatorStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public ModProcessor Build() => new(BackupManagerStub, ConfigStub, ModInteropStub, PluginLogStub, RuleEvaluatorStub);
}
