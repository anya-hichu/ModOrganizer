using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Backups.Fakes;
using ModOrganizer.Configs.Fakes;
using ModOrganizer.Mods;
using ModOrganizer.Mods.Fakes;
using ModOrganizer.Rules.Fakes;
using ModOrganizer.Tests.Configs;
using ModOrganizer.Tests.Mods.ModInterops;
using ModOrganizer.Tests.Rules.RuleEvaluators;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Backups;

namespace ModOrganizer.Tests.Mods.ModProcessors;

public class ModProcessorBuilder : Builder<ModProcessor>, IStubbableBackupManager, IStubbableConfig, IStubbableModInterop, IStubbablePluginLog, IStubbableRuleEvaluator
{
    public StubIBackupManager BackupManagerStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIConfig ConfigStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIModInterop ModInteropStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIRuleEvaluator RuleEvaluatorStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public override ModProcessor Build() => new(BackupManagerStub, ConfigStub, ModInteropStub, PluginLogStub, RuleEvaluatorStub);
}
