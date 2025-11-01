using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using ModOrganizer.Windows;
using ModOrganizer.Mods;
using ModOrganizer.Rules;

namespace ModOrganizer;

public sealed class Plugin : IDalamudPlugin
{
    public static readonly string NAMESPACE = "ModOrganizer"; 

    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IChatGui ChatGui { get; private set; } = null!;
    [PluginService] internal static IPluginLog PluginLog { get; private set; } = null!;

    private const string CommandName = "/modorganizer";
    private const string CommandHelpMessage = $"Available subcommands for {CommandName} are main, config";

    public Config Config { get; init; }

    public readonly WindowSystem WindowSystem = new(NAMESPACE);
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }


    private ModInterop ModInterop { get; init; }
    private RuleEvaluator RuleEvaluator { get; init; }
    private ModProcessor ModProcessor { get; init; }
    private ModAutoProcessor ModAutoProcessor { get; init; }

    public Plugin()
    {
        Config = PluginInterface.GetPluginConfig() as Config ?? new Config()
        {
            Rules = RuleBuilder.GetDefaults()
        };

        ModInterop = new(PluginInterface, PluginLog);
        RuleEvaluator = new(PluginLog);
        ModProcessor = new(Config, ModInterop, PluginLog, RuleEvaluator);
        ModAutoProcessor = new(ChatGui, Config, ModInterop, ModProcessor, PluginLog);

        ConfigWindow = new ConfigWindow(Config);
        MainWindow = new MainWindow(ModInterop);

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

    public void ToggleConfigUI() => ConfigWindow.Toggle();
    public void ToggleMainUI() => MainWindow.Toggle();
}
