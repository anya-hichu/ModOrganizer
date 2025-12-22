using Dalamud.Configuration;
using Dalamud.Interface.Fakes;
using Dalamud.Plugin.Fakes;
using Dalamud.Plugin.Ipc.Fakes;
using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using Penumbra.Api.Enums;
using Penumbra.Api.IpcSubscribers;

namespace ModOrganizer.Tests;

public class PluginBuilder
{
    public StubIUiBuilder UiBuilderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIDalamudPluginInterface PluginInterfaceStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubICommandManager CommandManagerStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public PluginBuilder() => PluginInterfaceStub.UiBuilderGet = () => UiBuilderStub;

    public PluginBuilder WithPluginLogDefaults()
    {
        PluginLogStub.BehaveAsDefaultValue();
        return this;
    }

    public PluginBuilder WithPluginInterfaceConfig(IPluginConfiguration? config)
    {
        PluginInterfaceStub.GetPluginConfig = () => config;
        return this;
    }

    public PluginBuilder WithPluginInterfaceConfigDirectory(DirectoryInfo configDirectory)
    {
        PluginInterfaceStub.ConfigDirectoryGet = () => configDirectory;
        return this;
    }

    public PluginBuilder WithPluginInterfaceInjectObjectFalse()
    {
        PluginInterfaceStub.InjectObjectObjectArray = (instance, scopedObjects) => false;
        return this;
    }

    public PluginBuilder WithCommandManagerAddHandlerTrue()
    {
        CommandManagerStub.AddHandlerStringCommandInfo = (command, info) => true;

        return this;
    }

    public PluginBuilder WithCommandManagerRemoveHandlerTrue()
    {
        CommandManagerStub.RemoveHandlerString = command => true;

        return this;
    }

    public PluginBuilder WithPenumbraModDirectory(DirectoryInfo modDirectory)
    {
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

        return this;
    }

    public PluginBuilder WithPenumbraModList(Dictionary<string, string> modList)
    {
        PluginInterfaceStub.GetIpcSubscriberOf1String(name => new StubICallGateSubscriber<Dictionary<string, string>>()
        {
            InvokeFunc = () =>
            {
                return name switch
                {
                    GetModList.Label => modList,
                    _ => throw new NotImplementedException(name),
                };
            }
        });

        return this;
    }

    public PluginBuilder WithPenumbraChangedItems(Dictionary<string, object?> changedItems)
    {
        PluginInterfaceStub.GetIpcSubscriberOf3String(name => new StubICallGateSubscriber<string, string, Dictionary<string, object?>>()
        {
            InvokeFuncT0T1 = (modDirectory, modName) =>
            {
                return name switch
                {
                    GetChangedItems.Label => changedItems,
                    _ => throw new NotImplementedException(name),
                };
            }
        });

        return this;
    }

    public PluginBuilder WithPenumbraSetModPath(PenumbraApiEc exitCode)
    {
        PluginInterfaceStub.GetIpcSubscriberOf4String(name => new StubICallGateSubscriber<string, string, string, int>()
        {
            InvokeFuncT0T1T2 = (modDirectory, newPath, modName) =>
            {
                return name switch
                {
                    SetModPath.Label => (int)exitCode,
                    _ => throw new NotImplementedException(name),
                };
            }
        });

        return this;
    }

    public PluginBuilder WithPenumbraModDeletedOrAddedNoop()
    {
        PluginInterfaceStub.GetIpcSubscriberOf2String(name => new StubICallGateSubscriber<string, object?>()
        {
            SubscribeActionOfT0 = action =>
            {
                switch (name)
                {
                    case ModAdded.Label or ModDeleted.Label: return;
                    default: throw new NotImplementedException(name);
                }
            }
        });

        return this;
    }

    public PluginBuilder WithPenumbraModMovedNoop()
    {
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

        return this;
    }

    public PluginBuilder WithPenumbraModDirectoryChangedNoop()
    {
        PluginInterfaceStub.GetIpcSubscriberOf3String(name => new StubICallGateSubscriber<string, bool, object?>()
        {
            SubscribeActionOfT0T1 = action =>
            {
                switch (name)
                {
                    case ModDirectoryChanged.Label: return;
                    default: throw new NotImplementedException(name);
                }
            }
        });

        return this;
    }

    public PluginBuilder WithPenumbraRegisterOrUnregisterSettingSectionNoop()
    {
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

        return this;
    }






    public Plugin Build()
    {
        Plugin.PluginLog = PluginLogStub;
        Plugin.PluginInterface = PluginInterfaceStub;
        Plugin.CommandManager = CommandManagerStub;

        return new();
    }
}
