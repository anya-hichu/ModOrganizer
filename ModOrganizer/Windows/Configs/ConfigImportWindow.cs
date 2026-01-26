using Dalamud.Bindings.ImGui;
using Dalamud.Interface.ImGuiFileDialog;
using Dalamud.Interface.ImGuiNotification;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using ModOrganizer.Configs;
using ModOrganizer.Configs.Mergers;
using ModOrganizer.Json.ConfigDatas;
using ModOrganizer.Json.Readers.Clipboards;
using ModOrganizer.Json.Readers.Files;
using ModOrganizer.Shared;
using ModOrganizer.Windows.Managers;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;

namespace ModOrganizer.Windows.Configs;

public class ConfigImportWindow : MultiWindow
{
    private IConverter<ConfigData, Config> ConfigDataConverter { get; init; }
    private IConfigMerger ConfigMerger { get; init; }
    private IConfigDataReader ConfigDataReader { get; init; }
    private INotificationManager NotificationManager { get; init; }

    private FileDialogManager FileDialogManager { get; init; } = new();
    private IConfig? MaybeImportConfig { get; set; }

    private bool Overwrite { get; set; } = false;

    public ConfigImportWindow(IConverter<ConfigData, Config> configDataConverter, IConfigMerger configMerger, IConfigDataReader configDataReader, INotificationManager notificationManager, IWindowManager windowManager) 
        : base("ModOrganizer - Config Import###configImportWindow", GenerateMonotonicId(), windowManager)
    {
        ConfigDataConverter = configDataConverter;
        ConfigMerger = configMerger;
        ConfigDataReader = configDataReader;
        NotificationManager = notificationManager;

        SizeConstraints = new()
        {
            MinimumSize = new(375, 330),
            MaximumSize = new(float.MaxValue, float.MaxValue)
        };
    }

    public override void Draw()
    {
        ImGui.Text("Load: ");
        ImGui.SameLine();
        if (ImGui.Button($"File###loadFile{SuffixId}")) FileDialogManager.OpenFileDialog("Import Config", "{.json}", LoadFromFileDialog);
        ImGui.SameLine();
        if (ImGui.Button($"Clipboard###loadClipboard{SuffixId}")) LoadFromClipboard(ImGui.GetClipboardText());

        FileDialogManager.Draw();

        if (MaybeImportConfig == null) return;

        // TODO Tree editor
        ImGui.Text(JsonSerializer.Serialize(MaybeImportConfig));

        ImGui.Text($"Conflicts: {ConfigMerger.GetConflicts(MaybeImportConfig).Count()}");

        var overwrite = Overwrite;
        if (ImGui.Checkbox($"Overwrite##overwrite{SuffixId}", ref overwrite)) Overwrite = overwrite;

        if (ImGui.Button($"Merge##merge{SuffixId}")) ConfigMerger.Merge(MaybeImportConfig, Overwrite);
    }

    private void LoadFromFileDialog(bool valid, string path)
    {
        if (!valid) return;
        LoadFromFile(path);
    }

    private void LoadFromFile(string path)
    {
        if (!ConfigDataReader.TryReadFromFile(path, out var configData))
        {
            NotifyError("Failed to read config from file");
            return;
        }
        
        if (!TryConvert(configData, out var config)) return;

        MaybeImportConfig = config;
    }

    private void LoadFromClipboard(string data)
    {
        if (data.IsNullOrWhitespace()) return;

        if (!ConfigDataReader.TryReadFromClipboard(data, out var configData))
        {
            NotifyError("Failed to read config from clipboard");
            return;
        }

        if (!TryConvert(configData, out var config)) return;

        MaybeImportConfig = config;
    }

    private bool TryConvert(ConfigData configData, [NotNullWhen(true)] out Config? config)
    {
        if (ConfigDataConverter.TryConvert(configData, out config)) return true;

        NotifyError("Failed to convert config from data");
        return false;
    }

    private void NotifyError(string content) => NotificationManager.AddNotification(new() { Type = NotificationType.Error, Title = nameof(ModOrganizer), Content = content });
}
