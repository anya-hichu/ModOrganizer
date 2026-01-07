using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Commands;
using ModOrganizer.Commands.Fakes;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Dalamuds.CommandManagers;

namespace ModOrganizer.Tests.Commands;

public class CommandBuilder : IBuilder<Command>, IStubbableCommandManager, IStubbableCommandPrinter
{
    public StubICommandManager CommandManagerStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubICommandPrinter CommandPrinterStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    private Action ToggleAboutWindow { get; set; } = () => throw new NotImplementedException();
    private Action ToggleBackupWindow { get; set; } = () => throw new NotImplementedException();
    private Action ToggleConfigWindow { get; set; } = () => throw new NotImplementedException();
    private Action ToggleConfigImportWindow { get; set; } = () => throw new NotImplementedException();
    private Action ToggleConfigExportWindow { get; set; } = () => throw new NotImplementedException();
    private Action ToggleMainWindow { get; set; } = () => throw new NotImplementedException();
    private Action TogglePreviewWindow { get; set; } = () => throw new NotImplementedException();

    public CommandBuilder WithToggleAboutWindow(Action value)
    {
        ToggleAboutWindow = value;
        return this;
    }

    public CommandBuilder WithToggleBackupWindow(Action value)
    {
        ToggleBackupWindow = value;
        return this;
    }

    public CommandBuilder WithToggleConfigWindow(Action value)
    {
        ToggleConfigWindow = value;
        return this;
    }

    public CommandBuilder WithToggleConfigImportWindow(Action value)
    {
        ToggleConfigImportWindow = value;
        return this;
    }

    public CommandBuilder WithToggleConfigExportWindow(Action value)
    {
        ToggleConfigExportWindow = value;
        return this;
    }

    public CommandBuilder WithToggleMainWindow(Action value)
    {
        ToggleMainWindow = value;
        return this;
    }

    public CommandBuilder WithTogglePreviewWindow(Action value)
    {
        TogglePreviewWindow = value;
        return this;
    }

    public Command Build() => new(CommandManagerStub, CommandPrinterStub, ToggleAboutWindow, ToggleBackupWindow, ToggleConfigWindow, ToggleConfigExportWindow, ToggleConfigImportWindow, ToggleMainWindow, TogglePreviewWindow);
}
