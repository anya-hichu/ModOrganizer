using Dalamud.Bindings.ImGui;
using Dalamud.Game.Command;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ModOrganizer.Mods;
using ModOrganizer.Rules;
using ModOrganizer.Windows;


namespace ModOrganizer;

public sealed class Plugin : IDalamudPlugin
{
    public static readonly string NAMESPACE = "ModOrganizer"; 

    [PluginService] public static IDalamudPluginInterface PluginInterface { get; set; } = null!;
    [PluginService] private static ICommandManager CommandManager { get; set; } = null!;
    [PluginService] private static IChatGui ChatGui { get; set; } = null!;
    [PluginService] private static IPluginLog PluginLog { get; set; } = null!;

    private const string CommandName = "/modorganizer";
    private const string CommandHelpMessage = $"Available subcommands for {CommandName} are main and config";

    public Config Config { get; init; }

    public readonly WindowSystem WindowSystem = new(NAMESPACE);
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }

    private ModInterop ModInterop { get; init; }
    private RuleEvaluator RuleEvaluator { get; init; }
    private ModProcessor ModProcessor { get; init; }
    private ModAutoProcessor ModAutoProcessor { get; init; }
    private ModVirtualFileSystem ModVirtualFileSystem { get; init; }

    public Plugin()
    {
        Config = PluginInterface.GetPluginConfig() as Config ?? new Config() { Rules = RuleBuilder.BuildDefaults() };

        ModInterop = new(PluginInterface, PluginLog);
        RuleEvaluator = new(PluginLog);
        ModProcessor = new(Config, ModInterop, PluginLog, RuleEvaluator);
        ModAutoProcessor = new(ChatGui, Config, ModInterop, ModProcessor, PluginLog);
        ModVirtualFileSystem = new(ModInterop);

        ConfigWindow = new ConfigWindow(Config)
        {
            TitleBarButtons = [new() { Icon = FontAwesomeIcon.ListAlt, ShowTooltip = () => ImGui.SetTooltip("Toggle Main Window"), Click = _ => ToggleMainUI() }]
        };
        MainWindow = new MainWindow(ModInterop, ModProcessor, ModVirtualFileSystem, PluginLog)
        {
            TitleBarButtons = [new() { Icon = FontAwesomeIcon.Cog, ShowTooltip = () => ImGui.SetTooltip("Toggle Config Window"), Click = _ => ToggleConfigUI() }]
        };
     
        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);

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

        ConfigWindow.Dispose();
        MainWindow.Dispose();

        CommandManager.RemoveHandler(CommandName);

        ModAutoProcessor.Dispose();
        ModInterop.Dispose();
        ModVirtualFileSystem.Dispose();
    }

    private void OnCommand(string command, string args)
    {
        var subcommand = args.Split(" ", 2)[0];
        if (subcommand == "main")
        {
            ToggleMainUI();
        }
        else if (subcommand == "config")
        {
            ToggleConfigUI();
        }
        else
        {
            ChatGui.Print(CommandHelpMessage);
        }
    }

    private void DrawUI() => WindowSystem.Draw();

    private void ToggleConfigUI() => ConfigWindow.Toggle();
    private void ToggleMainUI() => MainWindow.Toggle();
}
