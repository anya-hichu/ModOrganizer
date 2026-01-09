using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;

namespace ModOrganizer.Windows;

public class AboutWindow : Window
{
    public AboutWindow() : base("ModOrganizer - About##aboutWindow")
    {
        SizeConstraints = new()
        {
            MinimumSize = new(375, 330),
            MaximumSize = new(float.MaxValue, float.MaxValue)
        };
    }

    public override void Draw()
    {
        ImGui.Text("test");
    }
}
