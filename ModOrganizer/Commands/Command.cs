using Dalamud.Plugin.Services;
using System;

namespace ModOrganizer.Commands;

public class Command : ICommand
{
    public static readonly string NAME = "/modorganizer";
    public static readonly string HELP_MESSAGE = $"Available subcommands for {NAME} are about, backup, config (export|import)?, main and preview";

    private ICommandManager CommandManager { get; init; }
    private ICommandPrinter CommandPrinter { get; init; }
    private Action ToggleAboutWindow { get; init; }
    private Action ToggleBackupWindow { get; init; }
    private Action ToggleConfigWindow { get; init; }
    private Action ToggleConfigExportWindow { get; init; }
    private Action ToggleConfigImportWindow { get; init; }
    private Action ToggleMainWindow { get; init; }
    private Action TogglePreviewWindow { get; init; }

    public Command(ICommandManager commandManager, ICommandPrinter commandPrinter, Action toggleAboutWindow, Action toggleBackupWindow, Action toggleConfigWindow, Action toggleConfigExportWindow, 
        Action toggleConfigImportWindow, Action toggleMainWindow, Action togglePreviewWindow)
    {
        CommandManager = commandManager;
        CommandPrinter = commandPrinter;

        ToggleAboutWindow = toggleAboutWindow;
        ToggleBackupWindow = toggleBackupWindow;
        ToggleConfigWindow = toggleConfigWindow;
        ToggleConfigExportWindow = toggleConfigExportWindow;
        ToggleConfigImportWindow = toggleConfigImportWindow;
        ToggleMainWindow = toggleMainWindow;
        TogglePreviewWindow = togglePreviewWindow;

        CommandManager.AddHandler(NAME, new(Process)
        {
            HelpMessage = HELP_MESSAGE
        });
    }

    public void Dispose() => CommandManager.RemoveHandler(NAME);

    private void Process(string _, string subcommand)
    {
        switch (subcommand.Trim())
        {
            case "about":
                ToggleAboutWindow.Invoke();
                break;
            case "backup":
                ToggleBackupWindow.Invoke();
                break;
            case "config":
                ToggleConfigWindow.Invoke();
                break;
            case "config export":
                ToggleConfigExportWindow.Invoke();
                break;
            case "config import":
                ToggleConfigImportWindow.Invoke();
                break;
            case "main":
                ToggleMainWindow.Invoke();
                break;
            case "preview":
                TogglePreviewWindow.Invoke();
                break;
            default:
                CommandPrinter.PrintError(HELP_MESSAGE);
                break;
        }
    }
}
