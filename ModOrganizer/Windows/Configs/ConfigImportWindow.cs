using Dalamud.Interface.Windowing;

namespace ModOrganizer.Windows.Configs;

public class ConfigImportWindow : Window
{
    // Temporary Config like windows when can edit before deciding what to import (three state booleans and merge), edit paths, etc.
    // provide tree of rules with select boxes and possibility to rename folder/paths
    // Store in state the SelectedRules for merge (check conflicts)

    public ConfigImportWindow() : base("ModOrganizer - Config Import##configImportWindow")
    {
        SizeConstraints = new()
        {
            MinimumSize = new(375, 330),
            MaximumSize = new(float.MaxValue, float.MaxValue)
        };
    }

    public override void Draw()
    {
        throw new System.NotImplementedException();
    }
}
