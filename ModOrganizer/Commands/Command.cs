using Dalamud.Plugin.Services;
using System;

namespace ModOrganizer.Commands;

public class Command : ICommand
{
    private static readonly string COMMAND_NAME = "/modorganizer";
    private static readonly string COMMAND_HELP_MESSAGE = $"Available subcommands for {COMMAND_NAME} are about, backup, config (export|import)?, main and preview";

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

        CommandManager.AddHandler(COMMAND_NAME, new(Handler)
        {
            HelpMessage = COMMAND_HELP_MESSAGE
        });
    }

    public void Dispose() => CommandManager.RemoveHandler(COMMAND_NAME);

    private void Handler(string _, string subcommand)
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
                CommandPrinter.PrintError(COMMAND_HELP_MESSAGE);
                break;
        }
    }
}
