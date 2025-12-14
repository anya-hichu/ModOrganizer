using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ModOrganizer.Backups;
using ModOrganizer.Mods;
using ModOrganizer.Rules;
using ModOrganizer.Shared;
using ModOrganizer.Windows;
using ModOrganizer.Windows.States;
using System.IO;
using ThrottleDebounce;


namespace ModOrganizer;

public sealed class Plugin : IDalamudPlugin
{
    public static readonly string NAMESPACE = "ModOrganizer"; 

    [PluginService] private static IDalamudPluginInterface PluginInterface { get; set; } = null!;
    [PluginService] private static ICommandManager CommandManager { get; set; } = null!;
    [PluginService] private static IChatGui ChatGui { get; set; } = null!;
    [PluginService] private static IPluginLog PluginLog { get; set; } = null!;

    private const string CommandName = "/modorganizer";
    private const string CommandHelpMessage = $"Available subcommands for {CommandName} are main, config, preview and backup";

    private Config Config { get; init; }
    private ModInterop ModInterop { get; init; }
    private RuleEvaluator RuleEvaluator { get; init; }
    private ActionDebouncer ActionDebouncer { get; init; }
    private ModProcessor ModProcessor { get; init; }
    private ModAutoProcessor ModAutoProcessor { get; init; }
    private ModFileSystem ModFileSystem { get; init; }
    

    private WindowSystem WindowSystem { get; init; } = new(NAMESPACE);
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }
    private PreviewWindow PreviewWindow { get; init; }
    private BackupWindow BackupWindow { get; init; }

    public Plugin()
    {
        Config = PluginInterface.GetPluginConfig() as Config ?? new Config() { Rules = RuleBuilder.BuildDefaults() };

        ModInterop = new(PluginInterface, PluginLog);
        RuleEvaluator = new(PluginLog);

        var backupManager = new BackupManager(Config, ModInterop, PluginInterface, PluginLog);

        ActionDebouncer = new(PluginLog);
        ModProcessor = new(ActionDebouncer, backupManager, Config, ModInterop, PluginLog, RuleEvaluator);
        ModAutoProcessor = new(ChatGui, Config, ModInterop, ModProcessor, PluginLog);
        ModFileSystem = new(ModInterop);

        ConfigWindow = new(ActionDebouncer, Config, PluginInterface, ToggleMainUI);

        var ruleEvaluationState = new RuleEvaluationState(ModInterop, ModProcessor, PluginLog);
        MainWindow = new(Config, ModInterop, ModFileSystem, PluginLog, ruleEvaluationState, ToggleConfigUI, TogglePreviewUI, ToggleBackupUI);
        PreviewWindow = new(ruleEvaluationState);
        BackupWindow = new(backupManager, Config, ModInterop, PluginLog);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);
        WindowSystem.AddWindow(PreviewWindow);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = CommandHelpMessage
        });

        PluginInterface.UiBuilder.Draw += DrawUI;
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        MainWindow.Dispose();
        PreviewWindow.Dispose();

        CommandManager.RemoveHandler(CommandName);

        ModAutoProcessor.Dispose();
        ModInterop.Dispose();
        ModFileSystem.Dispose();
        ActionDebouncer.Dispose();
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
                ChatGui.Print(CommandHelpMessage);
                break;
        }
    }

    private void DrawUI() => WindowSystem.Draw();


    private void ToggleMainUI() => MainWindow.Toggle();
    private void ToggleConfigUI() => ConfigWindow.Toggle();
    private void TogglePreviewUI() => PreviewWindow.Toggle();
    private void ToggleBackupUI() => BackupWindow.Toggle();
}
