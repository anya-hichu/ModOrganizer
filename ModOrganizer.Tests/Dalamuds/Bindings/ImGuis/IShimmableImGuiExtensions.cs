using Dalamud.Bindings.ImGui;
using Dalamud.Bindings.ImGui.Fakes;
namespace ModOrganizer.Tests.Dalamuds.Bindings.ImGuis;

public static class IShimmableImGuiExtensions
{
    public static T WithImGuiInstance<T>(this T shimmable, IImGui instance) where T : IShimmableImGui
    {
        shimmable.OnShimsContext += () => ShimImGui.InstanceGet = () => instance;

        return shimmable;
    }
}
