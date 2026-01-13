using System;

namespace Dalamud.Bindings.ImGui;

public struct ImGuiListClipperPtr
{
    public IImGuiListClipperPtr? MaybeImplementation { get; set; }

    public readonly void Begin(int itemsCount, float itemsHeight) => GetImplementation().Begin(itemsCount, itemsHeight);
    public readonly void End() => GetImplementation().End();
    public readonly void Destroy() => GetImplementation().Destroy();

    private readonly IImGuiListClipperPtr GetImplementation() => MaybeImplementation ?? throw new NotImplementedException();
}
