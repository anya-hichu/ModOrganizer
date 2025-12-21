using Dalamud.Interface.Fakes;
using Dalamud.Plugin.Fakes;
using Dalamud.Plugin.Ipc.Fakes;
using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using Penumbra.Api.Enums;
using Penumbra.Api.IpcSubscribers;

namespace ModOrganizer.Tests;

[TestClass]
public sealed class TestPlugin
{
    private DirectoryInfo? ConfigDirectory { get; set; }
    private DirectoryInfo? PenumbraConfigDirectory { get; set; }

    [TestInitialize]
    public void Initialize()
    {
        ConfigDirectory = Directory.CreateDirectory(nameof(ModOrganizer));
        PenumbraConfigDirectory = Directory.CreateDirectory(nameof(Penumbra));

        Directory.CreateDirectory($"{PenumbraConfigDirectory.Name}/mod_data");

        var uiBuilder = new StubIUiBuilder()
        {
            InstanceBehavior = StubBehaviors.NotImplemented
        };

        var pluginInterfaceStub = new StubIDalamudPluginInterface()
        {
            GetPluginConfig = () => null,
            ConfigDirectoryGet = () => ConfigDirectory,
            InjectObjectObjectArray = (instance, scopedObjects) => false,
            UiBuilderGet = () => uiBuilder,
            InstanceBehavior = StubBehaviors.NotImplemented
        };

        var modDirectory = Directory.CreateDirectory("Mods");
        pluginInterfaceStub.GetIpcSubscriberOf1String(name => new StubICallGateSubscriber<string>()
        {
            InvokeFunc = () =>
            {
                return name switch
                {
                    GetModDirectory.Label => modDirectory.FullName,
                    _ => throw new NotImplementedException(name),
                };
            }
        });

        pluginInterfaceStub.GetIpcSubscriberOf1String(name => new StubICallGateSubscriber<Dictionary<string, string>>()
        {
            InvokeFunc = () =>
            {
                return name switch
                {
                    GetModList.Label => [],
                    _ => throw new NotImplementedException(name),
                };
            }
        });

        pluginInterfaceStub.GetIpcSubscriberOf3String(name => new StubICallGateSubscriber<string, string, Dictionary<string, object?>>()
        {
            InvokeFuncT0T1 = (modDirectory, modName) =>
            {
                return name switch
                {
                    GetChangedItems.Label => [],
                    _ => throw new NotImplementedException(name),
                };
            }
        });

        pluginInterfaceStub.GetIpcSubscriberOf4String(name => new StubICallGateSubscriber<string, string, string, int>()
        {
            InvokeFuncT0T1T2 = (modDirectory, newPath, modName) =>
            {
                return name switch
                {
                    SetModPath.Label => (int)PenumbraApiEc.Success,
                    _ => throw new NotImplementedException(name),
                };
            }
        });

        pluginInterfaceStub.GetIpcSubscriberOf2String(name => new StubICallGateSubscriber<string, object?>()
        {
            SubscribeActionOfT0 = action =>
            {
                switch (name)
                {
                    case ModAdded.Label: return;
                    case ModDeleted.Label: return;
                    default: throw new NotImplementedException(name);
                }
                ;
            }
        });

        pluginInterfaceStub.GetIpcSubscriberOf3String(name => new StubICallGateSubscriber<string, string, object?>()
        {
            SubscribeActionOfT0T1 = action =>
            {
                switch (name)
                {
                    case ModMoved.Label: return;
                    default: throw new NotImplementedException(name);
                }
                ;
            }
        });

        pluginInterfaceStub.GetIpcSubscriberOf3String(name => new StubICallGateSubscriber<string, bool, object?>()
        {
            SubscribeActionOfT0T1 = action =>
            {
                switch (name)
                {
                    case ModDirectoryChanged.Label: return;
                    default: throw new NotImplementedException(name);
                }
                ;
            }
        });

        pluginInterfaceStub.GetIpcSubscriberOf2String(name => new StubICallGateSubscriber<Action, int>()
        {
            InvokeFuncT0 = action =>
            {
                return name switch
                {
                    RegisterSettingsSection.Label or UnregisterSettingsSection.Label => (int)PenumbraApiEc.Success,
                    _ => throw new NotImplementedException(name)
                };
            }
        });


        Plugin.PluginInterface = pluginInterfaceStub;
        Plugin.CommandManager = new StubICommandManager()
        {
            AddHandlerStringCommandInfo = (command, commandInfo) => true,
            RemoveHandlerString = (command) => true,
            InstanceBehavior = StubBehaviors.NotImplemented
        };

        Plugin.PluginLog = new StubIPluginLog() { InstanceBehavior = StubBehaviors.DefaultValue };
    }


    [TestMethod]
    public void TestNew()
    {
        var plugin = new Plugin();
        Assert.IsNotNull(plugin);
    }

    [TestCleanup]
    public void Cleanup()
    {
        Plugin.PluginInterface = null!;
        Plugin.CommandManager = null!;
        Plugin.PluginLog = null!;

        ConfigDirectory?.Delete(true);
        PenumbraConfigDirectory?.Delete(true);
    }
}
