namespace Dalamud.Bindings.ImGui;

public interface IImGuiListClipperPtr
{
    void Begin(int itemsCount, float itemsHeight);
    void End();
    void Destroy();
}
