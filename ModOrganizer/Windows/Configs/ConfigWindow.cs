using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using ModOrganizer.Configs;
using ModOrganizer.Providers;
using ModOrganizer.Rules;
using ModOrganizer.Windows.Togglers;
using Penumbra.Api.IpcSubscribers;
using System;
using System.Linq;
using ThrottleDebounce;

namespace ModOrganizer.Windows.Configs;

public class ConfigWindow : Window
{
    private IConfig Config { get; init; }
    private IDalamudPluginInterface PluginInterface { get; init; }
    private IPluginProvider PluginProvider { get; init; }
    private IWindowToggler WindowToggler { get; init; }
    

    private RateLimitedAction SaveConfigLaterAction { get; init; }

    private RegisterSettingsSection RegisterSettingsSection { get; init; }
    private UnregisterSettingsSection UnregisterSettingsSection { get; init; }

    public ConfigWindow(IConfig config, IDalamudPluginInterface pluginInterface, IPluginProvider pluginProvider, IWindowToggler windowToggler) : base("ModOrganizer - Config##configWindow")
    {
        Config = config;
        PluginInterface = pluginInterface;
        PluginProvider = pluginProvider;
        WindowToggler = windowToggler;

        SizeConstraints = new()
        {
            MinimumSize = new(375, 330),
            MaximumSize = new(float.MaxValue, float.MaxValue)
        };

        TitleBarButtons = [
            new() {
                Icon = FontAwesomeIcon.Database,
                ShowTooltip = () => ImGui.SetTooltip("Toggle backup window"),
                Click = _ => WindowToggler.Toggle<BackupWindow>()
            }
        ];

        RegisterSettingsSection = new(pluginInterface);
        UnregisterSettingsSection = new(pluginInterface);

        RegisterSettingsSection.Invoke(DrawAutoProcessSettings);

        SaveConfigLaterAction = Debouncer.Debounce(SaveConfig, TimeSpan.FromSeconds(1));
    }

    public void Dispose() 
    {
        SaveConfigLaterAction.Dispose();
        UnregisterSettingsSection.Invoke(DrawAutoProcessSettings);
    } 

    public override void Draw()
    {
        if (ImGui.Button("Import##import")) PluginProvider.Init<ConfigImportWindow>();

        if (ImGui.Button("Export##export")) PluginProvider.Init<ConfigExportWindow>();

        DrawAutoProcessSettings();

        var autoBackupEnabled = Config.AutoBackupEnabled;
        if (ImGui.Checkbox("Auto-Backup (recommended)##autoBackupEnabled", ref autoBackupEnabled))
        {
            Config.AutoBackupEnabled = autoBackupEnabled;
            SaveConfig();
        }

        var autoBackupLimit = Config.AutoBackupLimit;
        if (ImGui.InputUShort("Auto-Backup Limit##autoBackupLimit", ref autoBackupLimit, 1))
        {
            Config.AutoBackupLimit = autoBackupLimit;
            SaveConfigLater();
        }

        if (ImGui.Button("New rule##newRule"))
        {
            Config.Rules.Add(new());
            SaveConfig();
        }

        foreach (var rule in Config.Rules.OrderDescending())
        {
            var hash = rule.GetHashCode();

            var enabled = rule.Enabled;
            if (ImGui.Checkbox($"Enable##rule{hash}Enabled", ref enabled))
            {
                rule.Enabled = enabled;
                SaveConfig();
            }

            var path = rule.Path;
            if (ImGui.InputText($"Name##rule{hash}Path", ref path))
            {
                if (Config.Rules.Contains(new Rule { Path = path }))
                {
                    // Todo conflicts resolution popup to override

                }
                else
                {
                    
                    rule.Path = path;
                    SaveConfigLater();
                }  
            }

            var priority = rule.Priority;
            if (ImGui.InputInt($"Priority##rule{hash}Priority", ref priority, 1, 1))
            {
                rule.Priority = priority;
                SaveConfig();
            }

            var matchExpression = rule.MatchExpression;
            if (ImGui.InputTextMultiline($"Match expression##rule{hash}MatchExpression", ref matchExpression, ushort.MaxValue))
            {
                rule.MatchExpression = matchExpression;
                SaveConfigLater();
            }

            var pathTemplate = rule.PathTemplate;
            if (ImGui.InputTextMultiline($"Path template##rule{hash}PathTemplate", ref pathTemplate, ushort.MaxValue))
            {
                rule.PathTemplate = pathTemplate;
                SaveConfigLater();
            }
        }
    }

    private void DrawAutoProcessSettings()
    {
        var autoProcessEnabled = Config.AutoProcessEnabled;
        if (ImGui.Checkbox("Auto-Process##autoProcessEnabled", ref autoProcessEnabled))
        {
            Config.AutoProcessEnabled = autoProcessEnabled;
            SaveConfig();
        }
        // TODO add info that only added mods are auto-processed

        var autoProcessDelayMs = Config.AutoProcessDelayMs;
        if (ImGui.InputUShort("Auto-Process Delay (ms)##autoProcessDelayMs", ref autoProcessDelayMs, 50))
        {
            Config.AutoProcessDelayMs = autoProcessDelayMs;
            SaveConfigLater();
        }
    }

    private void SaveConfig() => PluginInterface.SavePluginConfig(Config);
    private void SaveConfigLater() => SaveConfigLaterAction.Invoke();
}
