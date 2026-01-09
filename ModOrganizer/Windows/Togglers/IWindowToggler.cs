using Dalamud.Interface.Windowing;

namespace ModOrganizer.Windows.Togglers;

public interface IWindowToggler
{
    void SetWindowSystem(WindowSystem windowSystem);

    void Toggle<T>() where T : Window;
}
