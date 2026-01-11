namespace Dalamud.Bindings.ImGui;

public struct ImGuiListClipperPtr
{
    public IImGuiListClipperPtr? MaybeImplementation { get; set; }

    public readonly void Begin(int itemsCount, float itemsHeight) => MaybeImplementation!.Begin(itemsCount, itemsHeight);
    public readonly void End() => MaybeImplementation!.End();
    public readonly void Destroy() => MaybeImplementation!.Destroy();
}
