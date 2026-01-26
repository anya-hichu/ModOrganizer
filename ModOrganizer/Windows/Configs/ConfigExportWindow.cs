using ModOrganizer.Configs;
using ModOrganizer.Configs.Loaders;
using ModOrganizer.Windows.Managers;
using System;

namespace ModOrganizer.Windows.Configs;

public class ConfigExportWindow : MultiWindow
{
    private IConfig ExportConfig { get; init; }

    public ConfigExportWindow(IConfigLoader configLoader, IWindowManager windowManager) : base("ModOrganizer - Config Export###configExportWindow", GenerateMonotonicId(), windowManager)
    {
        ExportConfig = configLoader.GetOrDefault();

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
