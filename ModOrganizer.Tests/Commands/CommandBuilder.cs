using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Commands;
using ModOrganizer.Commands.Fakes;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Dalamuds.CommandManagers;
using ModOrganizer.Tests.Windows.Togglers;
using ModOrganizer.Windows.Togglers.Fakes;

namespace ModOrganizer.Tests.Commands;

public class CommandBuilder : IBuilder<Command>, IStubbableCommandManager, IStubbableCommandPrinter, IStubbableWindowToggler
{
    public StubICommandManager CommandManagerStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubICommandPrinter CommandPrinterStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIWindowToggler WindowTogglerStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public Command Build() => new(CommandManagerStub, CommandPrinterStub, WindowTogglerStub);
}
