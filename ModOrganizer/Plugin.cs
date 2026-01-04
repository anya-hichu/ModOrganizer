using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ModOrganizer.Backups;
using ModOrganizer.Configs;
using ModOrganizer.Json.Penumbra.DefaultMods;
using ModOrganizer.Json.Penumbra.Groups;
using ModOrganizer.Json.Penumbra.LocalModDatas;
using ModOrganizer.Json.Penumbra.ModMetas;
using ModOrganizer.Json.Penumbra.SortOrders;
using ModOrganizer.Json.Readers;
using ModOrganizer.Mods;
using ModOrganizer.Rules;
using ModOrganizer.Windows;
using ModOrganizer.Windows.Configs;
using ModOrganizer.Windows.States;

namespace ModOrganizer;

public sealed class Plugin : IDalamudPlugin
{
    private static readonly string COMMAND_NAME = "/modorganizer";
    private static readonly string COMMAND_HELP_MESSAGE = $"Available subcommands for [{COMMAND_NAME}] are about, backup, config (export|import)?, main and preview";

    [PluginService] public static IChatGui ChatGui { get; set; } = null!;
    [PluginService] public static ICommandManager CommandManager { get; set; } = null!;
    [PluginService] public static INotificationManager NotificationManager { get; set; } = null!;
    [PluginService] public static IDalamudPluginInterface PluginInterface { get; set; } = null!;
    [PluginService] public static IPluginLog PluginLog { get; set; } = null!;

    private Config Config { get; init; }
    private ReaderProvider ReaderProvider { get; init; }
    private ModInterop ModInterop { get; init; }
    private RuleEvaluator RuleEvaluator { get; init; }
    private ModProcessor ModProcessor { get; init; }
    private ModAutoProcessor ModAutoProcessor { get; init; }
    private ModFileSystem ModFileSystem { get; init; }
    private BackupManager BackupManager { get; init; }

    private RuleState RuleState { get; init; }

    private WindowSystem WindowSystem { get; init; }

    private AboutWindow AboutWindow { get; init; }
    private BackupWindow BackupWindow { get; init; }
    private ConfigWindow ConfigWindow { get; init; }
    private ConfigExportWindow ConfigExportWindow { get; init; }
    private ConfigImportWindow ConfigImportWindow { get; init; }
    private MainWindow MainWindow { get; init; }
    private PreviewWindow PreviewWindow { get; init; }
    


    public Plugin()
    {
        Config = PluginInterface.GetPluginConfig() as Config ?? ConfigBuilder.BuildDefault();

        ReaderProvider = new(PluginLog);

        var sortOrderReader = ReaderProvider.Get<ISortOrderReader>();
        ModInterop = new(CommandManager, ReaderProvider.Get<IDefaultModReader>(), ReaderProvider.Get<IGroupReaderFactory>(),
            ReaderProvider.Get<ILocalModDataReader>(), ReaderProvider.Get<IModMetaReader>(), PluginInterface, PluginLog, sortOrderReader);

        RuleEvaluator = new(PluginLog);
        BackupManager = new(Config, ModInterop, PluginInterface, PluginLog, sortOrderReader);
        ModProcessor = new(BackupManager, Config, ModInterop, PluginLog, RuleEvaluator);
        ModAutoProcessor = new(Config, NotificationManager, ModInterop, ModProcessor, PluginLog);
        ModFileSystem = new(ModInterop);

        RuleState = new(Config, BackupManager, ModInterop, ModProcessor, PluginLog);

        WindowSystem = new(nameof(ModOrganizer));

        AboutWindow = new();
        BackupWindow = new(BackupManager, Config, ModInterop, PluginLog);
        ConfigWindow = new(Config, PluginInterface, ToggleBackupUI, ToggleMainUI);
        ConfigExportWindow = new();
        ConfigImportWindow = new();
        MainWindow = new(Config, ModInterop, ModFileSystem, PluginLog, RuleState, ToggleBackupUI, ToggleConfigUI, TogglePreviewUI);
        PreviewWindow = new(RuleState);

        WindowSystem.AddWindow(AboutWindow);
        WindowSystem.AddWindow(BackupWindow);
        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(ConfigExportWindow);
        WindowSystem.AddWindow(ConfigImportWindow);
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

        ReaderProvider.Dispose();
    }

    private void OnCommand(string _, string subcommand)
    {
        switch (subcommand.Trim())
        {
            case "auto":
                ToggleAboutUI();
                break;
            case "backup":
                ToggleBackupUI();
                break;
            case "config":
                ToggleConfigUI();
                break;
            case "config export":
                ToggleConfigExportUI();
                break;
            case "config import":
                ToggleConfigImportUI();
                break;
            case "main":
                ToggleMainUI();
                break;
            case "preview":
                TogglePreviewUI();
                break;
            default:
                ChatGui.Print(COMMAND_HELP_MESSAGE);
                break;
        }
    }

    private void DrawUI() => WindowSystem.Draw();

    private void ToggleAboutUI() => AboutWindow.Toggle();
    private void ToggleBackupUI() => BackupWindow.Toggle();
    private void ToggleConfigUI() => ConfigWindow.Toggle();
    private void ToggleConfigExportUI() => ConfigExportWindow.Toggle();
    private void ToggleConfigImportUI() => ConfigImportWindow.Toggle();
    private void ToggleMainUI() => MainWindow.Toggle();
    private void TogglePreviewUI() => PreviewWindow.Toggle();
}
