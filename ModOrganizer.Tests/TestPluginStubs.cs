using Dalamud.Interface.Fakes;
using Dalamud.Plugin.Fakes;
using Dalamud.Plugin.Ipc.Fakes;
using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using Penumbra.Api.Enums;
using Penumbra.Api.IpcSubscribers;

namespace ModOrganizer.Tests;

public class TestPluginStubs
{
    public StubIUiBuilder UiBuilderStub { get; init; }
    public StubIDalamudPluginInterface  PluginInterfaceStub { get; init; }
    public StubICommandManager CommandManagerStub { get; init; }
    public StubIPluginLog PluginLogStub { get; init; }

    public TestPluginStubs(string tempDirectory)
    {
        var configDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer)));
        var penumbraConfigDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(Penumbra)));
        Directory.CreateDirectory(Path.Combine(penumbraConfigDirectory.FullName, "mod_data"));

        UiBuilderStub = new() { InstanceBehavior = StubBehaviors.NotImplemented };

        PluginInterfaceStub = new()
        {
            GetPluginConfig = () => null,
            ConfigDirectoryGet = () => configDirectory,
            InjectObjectObjectArray = (instance, scopedObjects) => false,
            UiBuilderGet = () => UiBuilderStub,
            InstanceBehavior = StubBehaviors.NotImplemented
        };

        var modDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, "Mods"));
        PluginInterfaceStub.GetIpcSubscriberOf1String(name => new StubICallGateSubscriber<string>()
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

        PluginInterfaceStub.GetIpcSubscriberOf1String(name => new StubICallGateSubscriber<Dictionary<string, string>>()
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

        PluginInterfaceStub.GetIpcSubscriberOf3String(name => new StubICallGateSubscriber<string, string, Dictionary<string, object?>>()
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

        PluginInterfaceStub.GetIpcSubscriberOf4String(name => new StubICallGateSubscriber<string, string, string, int>()
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

        PluginInterfaceStub.GetIpcSubscriberOf2String(name => new StubICallGateSubscriber<string, object?>()
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

        PluginInterfaceStub.GetIpcSubscriberOf3String(name => new StubICallGateSubscriber<string, string, object?>()
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

        PluginInterfaceStub.GetIpcSubscriberOf3String(name => new StubICallGateSubscriber<string, bool, object?>()
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

        PluginInterfaceStub.GetIpcSubscriberOf2String(name => new StubICallGateSubscriber<Action, int>()
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

        CommandManagerStub = new()
        {
            AddHandlerStringCommandInfo = (command, commandInfo) => true,
            RemoveHandlerString = (command) => true,
            InstanceBehavior = StubBehaviors.NotImplemented
        };

        PluginLogStub = new() { InstanceBehavior = StubBehaviors.DefaultValue };
    }
}
