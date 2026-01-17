using Microsoft.QualityTools.Testing.Fakes.Stubs;
using Dalamud.Bindings.ImGui.Fakes;

namespace ModOrganizer.Tests.Dalamuds.Bindings.ImGuis;

public static class IStubbableImGuiExtensions
{
    public static T WithImGuiDefaults<T>(this T stubbable) where T : IStubbableImGui
    {
        stubbable.ImGuiStub.BehaveAsDefaultValue();

        return stubbable;
    }

    public static T WithImGuiObserver<T>(this T stubbable, IStubObserver observer) where T : IStubbableImGui
    {
        stubbable.ImGuiStub.InstanceObserver = observer;

        return stubbable;
    }

    public static T WithImGuiListClipper<T>(this T stubbable, StubIImGuiListClipperPtr stubValue) where T : IStubbableImGui
    {
        stubbable.ImGuiStub.ImGuiListClipper = () => new()
        {
            MaybeImplementation = stubValue
        };

        return stubbable;
    }

    public static T WithImGuiTreeNodeEx<T>(this T stubbable, bool stubValue) where T : IStubbableImGui
    {
        stubbable.ImGuiStub.TreeNodeExImU8StringImGuiTreeNodeFlagsImU8String = (id, flags, label) => stubValue;

        return stubbable;
    }
}
