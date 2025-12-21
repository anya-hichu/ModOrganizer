using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using ModOrganizer.Configs;

namespace ModOrganizer.Windows.Configs;

public class ConfigImportWindow : Window
{
    // Temporary Config like windows when can edit before deciding what to import (three state booleans and merge), edit paths, etc.
    // provide tree of rules with select boxes and possibility to rename folder/paths
    // Store in state the SelectedRules for merge (check conflicts)

    private Config Config { get; init; }

    public ConfigImportWindow(Config config, IDalamudPluginInterface pluginInterface) : base("ModOrganizer - Config Import##configImportWindow")
    {
        SizeConstraints = new()
        {
            MinimumSize = new(375, 330),
            MaximumSize = new(float.MaxValue, float.MaxValue)
        };

        Config = config;

    }

    public override void Draw()
    {
        throw new System.NotImplementedException();
    }
}
