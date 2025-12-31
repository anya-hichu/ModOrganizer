using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ModOrganizer.Backups;
using ModOrganizer.Configs;
using ModOrganizer.Mods;
using ModOrganizer.Rules;
using ModOrganizer.Windows;
using ModOrganizer.Windows.Configs;
using ModOrganizer.Windows.States;

namespace ModOrganizer;

public sealed class Plugin : IDalamudPlugin
{
    private static readonly string COMMAND_NAME = "/modorganizer";
    private static readonly string COMMAND_HELP_MESSAGE = $"Available subcommands for {COMMAND_NAME} are main, config, preview and backup";

    [PluginService] public static IChatGui ChatGui { get; set; } = null!;
    [PluginService] public static ICommandManager CommandManager { get; set; } = null!;
    [PluginService] public static INotificationManager NotificationManager { get; set; } = null!;
    [PluginService] public static IDalamudPluginInterface PluginInterface { get; set; } = null!;
    [PluginService] public static IPluginLog PluginLog { get; set; } = null!;

    private Config Config { get; init; }
    private ModInterop ModInterop { get; init; }
    private RuleEvaluator RuleEvaluator { get; init; }
    private ModProcessor ModProcessor { get; init; }
    private ModAutoProcessor ModAutoProcessor { get; init; }
    private ModFileSystem ModFileSystem { get; init; }
    private BackupManager BackupManager { get; init; }

    private RuleState RuleState { get; init; }

    private WindowSystem WindowSystem { get; init; }
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }
    private PreviewWindow PreviewWindow { get; init; }
    private BackupWindow BackupWindow { get; init; }

    public Plugin()
    {
        Config = PluginInterface.GetPluginConfig() as Config ?? ConfigBuilder.BuildDefault();

        ModInterop = new(CommandManager, PluginInterface, PluginLog);

        RuleEvaluator = new(PluginLog);

        BackupManager = new(Config, ModInterop, PluginInterface, PluginLog);

        ModProcessor = new(BackupManager, Config, ModInterop, PluginLog, RuleEvaluator);
        ModAutoProcessor = new(Config, NotificationManager, ModInterop, ModProcessor, PluginLog);
        ModFileSystem = new(ModInterop);

        RuleState = new(Config, BackupManager, ModInterop, ModProcessor, PluginLog);

        WindowSystem = new(nameof(ModOrganizer));
        ConfigWindow = new(Config, PluginInterface, ToggleBackupUI, ToggleMainUI);
        MainWindow = new(Config, ModInterop, ModFileSystem, PluginLog, RuleState, ToggleBackupUI, ToggleConfigUI, TogglePreviewUI);
        PreviewWindow = new(RuleState);
        BackupWindow = new(BackupManager, Config, ModInterop, PluginLog);

        WindowSystem.AddWindow(BackupWindow);
        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);
        WindowSystem.AddWindow(PreviewWindow);

        CommandManager.AddHandler(COMMAND_NAME, new CommandInfo(OnCommand)
        {
            HelpMessage = COMMAND_HELP_MESSAGE
        });

        PluginInterface.UiBuilder.Draw += DrawUI;
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        BackupWindow.Dispose();
        ConfigWindow.Dispose();
        MainWindow.Dispose();
        PreviewWindow.Dispose();

        CommandManager.RemoveHandler(COMMAND_NAME);

        ModAutoProcessor.Dispose();
        ModInterop.Dispose();
        ModFileSystem.Dispose();

        RuleState.Dispose();
        BackupManager.Dispose();
    }

    private void OnCommand(string command, string subcommand)
    {
        switch (subcommand)
        {
            case "main":
                ToggleMainUI();
                break;
            case "config":
                ToggleConfigUI();
                break;
            case "preview":
                TogglePreviewUI();
                break;
            case "backup":
                ToggleBackupUI();
                break;
            default:
                ChatGui.Print(COMMAND_HELP_MESSAGE);
                break;
        }
    }

    private void DrawUI() => WindowSystem.Draw();


    private void ToggleMainUI() => MainWindow.Toggle();
    private void ToggleConfigUI() => ConfigWindow.Toggle();
    private void TogglePreviewUI() => PreviewWindow.Toggle();
    private void ToggleBackupUI() => BackupWindow.Toggle();
}
