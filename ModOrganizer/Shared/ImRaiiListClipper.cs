using Dalamud.Bindings.ImGui;
using System;

namespace ModOrganizer.Shared;

public class ImRaiiListClipper(int itemsCount, float itemsHeight) : RaiiGuard<ImGuiListClipperPtr>(BuildAcquire(itemsCount, itemsHeight), Release)
{
    private static Func<ImGuiListClipperPtr> BuildAcquire(int itemsCount, float itemsHeight) => () => 
    {
        var clipper = ImGui.ImGuiListClipper();
        clipper.Begin(itemsCount, itemsHeight);
        return clipper;
    }; 

    private static void Release(ImGuiListClipperPtr clipper)
    {
        clipper.End();
        clipper.Destroy();
    }
}
