using Dalamud.Interface.Windowing;

namespace ModOrganizer.Windows.Togglers;

public interface IWindowToggler
{
    WindowSystem? MaybeWindowSystem { get; set; }

    void Toggle<T>() where T : Window;
}
