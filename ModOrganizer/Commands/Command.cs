using Dalamud.Plugin.Services;
using ModOrganizer.Windows;
using ModOrganizer.Windows.Configs;
using ModOrganizer.Windows.Togglers;

namespace ModOrganizer.Commands;

public class Command : ICommand
{
    public static readonly string NAME = "/modorganizer";
    public static readonly string HELP_MESSAGE = $"Available subcommands for {NAME} are about, backup, config (export|import)?, main and preview";

    private ICommandManager CommandManager { get; init; }
    private ICommandPrinter CommandPrinter { get; init; }
    private IWindowToggler WindowToggler { get; init; }

    public Command(ICommandManager commandManager, ICommandPrinter commandPrinter, IWindowToggler windowToggler)
    {
        CommandManager = commandManager;
        CommandPrinter = commandPrinter;
        WindowToggler = windowToggler;

        CommandManager.AddHandler(NAME, new(Process)
        {
            HelpMessage = HELP_MESSAGE
        });
    }

    public void Dispose() => CommandManager.RemoveHandler(NAME);

    private void Process(string _, string arguments)
    {
        switch (arguments.Trim())
        {
            case "about":
                WindowToggler.Toggle<AboutWindow>();
                break;
            case "backup":
                WindowToggler.Toggle<BackupWindow>();
                break;
            case "config":
                WindowToggler.Toggle<ConfigWindow>();
                break;
            case "config export":
                WindowToggler.Toggle<ConfigExportWindow>();
                break;
            case "config import":
                WindowToggler.Toggle<ConfigImportWindow>();
                break;
            case "main":
                WindowToggler.Toggle<MainWindow>();
                break;
            case "preview":
                WindowToggler.Toggle<PreviewWindow>();
                break;
            default:
                CommandPrinter.PrintError(HELP_MESSAGE);
                break;
        }
    }
}
