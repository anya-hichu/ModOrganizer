using Dalamud.Interface.Windowing;

namespace ModOrganizer.Windows.Managers;

public interface IWindowManager
{
    WindowSystem? MaybeWindowSystem { get; set; }

    void Add(Window window);
    void Remove(Window window);
}
