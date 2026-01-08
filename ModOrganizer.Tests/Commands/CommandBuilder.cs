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

    public CommandBuilder WithToggleAboutWindow(Action stubAction)
    {
        ToggleAboutWindow = stubAction;
        return this;
    }

    public CommandBuilder WithToggleBackupWindow(Action stubAction)
    {
        ToggleBackupWindow = stubAction;
        return this;
    }

    public CommandBuilder WithToggleConfigWindow(Action stubAction)
    {
        ToggleConfigWindow = stubAction;
        return this;
    }

    public CommandBuilder WithToggleConfigImportWindow(Action stubAction)
    {
        ToggleConfigImportWindow = stubAction;
        return this;
    }

    public CommandBuilder WithToggleConfigExportWindow(Action stubAction)
    {
        ToggleConfigExportWindow = stubAction;
        return this;
    }

    public CommandBuilder WithToggleMainWindow(Action stubAction)
    {
        ToggleMainWindow = stubAction;
        return this;
    }

    public CommandBuilder WithTogglePreviewWindow(Action stubAction)
    {
        TogglePreviewWindow = stubAction;
        return this;
    }

    public Command Build() => new(CommandManagerStub, CommandPrinterStub, ToggleAboutWindow, ToggleBackupWindow, ToggleConfigWindow, 
        ToggleConfigExportWindow, ToggleConfigImportWindow, ToggleMainWindow, TogglePreviewWindow);
}
