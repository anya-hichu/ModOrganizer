using Dalamud.Configuration;
using Dalamud.Interface.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;

namespace ModOrganizer.Tests.Dalamuds.PluginInterfaces;

public static class IStubbablePluginInterfaceExtensions
{
    public static T WithPluginInterfaceUiBuilderStub<T>(this T stubbable) where T : IStubbablePluginInterface
    {
        stubbable.PluginInterfaceStub.UiBuilderGet = () => new StubIUiBuilder() { InstanceBehavior = StubBehaviors.NotImplemented };
    
        return stubbable;
    }

    public static T WithPluginInterfaceConfig<T>(this T stubbable, IPluginConfiguration? config) where T : IStubbablePluginInterface
    {
        stubbable.PluginInterfaceStub.GetPluginConfig = () => config;

        return stubbable;
    }

    public static T WithPluginInterfaceConfigDirectory<T>(this T stubbable, DirectoryInfo configDirectory) where T : IStubbablePluginInterface
    {
        stubbable.PluginInterfaceStub.ConfigDirectoryGet = () => configDirectory;

        return stubbable;
    }

    public static T WithPluginInterfaceSaveConfigNoop<T>(this T stubbable) where T : IStubbablePluginInterface
    {
        stubbable.PluginInterfaceStub.SavePluginConfigIPluginConfiguration = config => { };

        return stubbable;
    }

    public static T WithPluginInterfaceInjectObject<T>(this T stubbable, bool stubValue) where T : IStubbablePluginInterface
    {
        stubbable.PluginInterfaceStub.InjectObjectObjectArray = (instance, scopedObjects) => stubValue;

        return stubbable;
    }
}
