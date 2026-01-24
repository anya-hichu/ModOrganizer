using Dalamud.Bindings.ImGui;
using ModOrganizer.Configs;
using ModOrganizer.Configs.Mergers;
using ModOrganizer.Windows.Managers;

namespace ModOrganizer.Windows.Configs;

public class ConfigImportWindow : ConfigDataWindow
{
    private IConfigMerger ConfigMerger { get; init; }
    private IConfig? MaybeImportConfig { get; set; }

    public ConfigImportWindow(IConfigMerger configMerger, IWindowManager windowManager) : base("ModOrganizer - Config Import###configImportWindow", windowManager)
    {
        ConfigMerger = configMerger;

        SizeConstraints = new()
        {
            MinimumSize = new(375, 330),
            MaximumSize = new(float.MaxValue, float.MaxValue)
        };
    }

    public override void Draw()
    {
        if (ImGui.Button("Import File##importFile")) MaybeImportConfig = null;
        ImGui.SameLine();
        if (ImGui.Button("Import Clipboard##importClipboard")) MaybeImportConfig = null;
    }
}
