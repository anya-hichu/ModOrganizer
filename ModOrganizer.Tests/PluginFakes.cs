using Dalamud.Interface.Fakes;
using Dalamud.Plugin.Fakes;
using Dalamud.Plugin.Ipc.Fakes;
using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using Penumbra.Api.Enums;
using Penumbra.Api.IpcSubscribers;

namespace ModOrganizer.Tests;

public class PluginFakes
{
    public StubIUiBuilder UiBuilder { get; init; }
    public StubIDalamudPluginInterface  PluginInterface { get; init; }
    public StubICommandManager CommandManager { get; init; }
    public StubIPluginLog PluginLog { get; init; }

    public PluginFakes(string tempDirectory)
    {
        var configDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer)));
        var penumbraConfigDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(Penumbra)));
        Directory.CreateDirectory(Path.Combine(penumbraConfigDirectory.FullName, "mod_data"));

        UiBuilder = new() { InstanceBehavior = StubBehaviors.NotImplemented };

        PluginInterface = new()
        {
            GetPluginConfig = () => null,
            ConfigDirectoryGet = () => configDirectory,
            InjectObjectObjectArray = (instance, scopedObjects) => false,
            UiBuilderGet = () => UiBuilder,
            InstanceBehavior = StubBehaviors.NotImplemented
        };

        var modDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, "Mods"));
        PluginInterface.GetIpcSubscriberOf1String(name => new StubICallGateSubscriber<string>()
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

        PluginInterface.GetIpcSubscriberOf1String(name => new StubICallGateSubscriber<Dictionary<string, string>>()
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

        PluginInterface.GetIpcSubscriberOf3String(name => new StubICallGateSubscriber<string, string, Dictionary<string, object?>>()
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

        PluginInterface.GetIpcSubscriberOf4String(name => new StubICallGateSubscriber<string, string, string, int>()
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

        PluginInterface.GetIpcSubscriberOf2String(name => new StubICallGateSubscriber<string, object?>()
        {
            SubscribeActionOfT0 = action =>
            {
                switch (name)
                {
                    case ModAdded.Label or ModDeleted.Label: return;
                    default: throw new NotImplementedException(name);
                }
                ;
            }
        });

        PluginInterface.GetIpcSubscriberOf3String(name => new StubICallGateSubscriber<string, string, object?>()
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

        PluginInterface.GetIpcSubscriberOf3String(name => new StubICallGateSubscriber<string, bool, object?>()
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

        PluginInterface.GetIpcSubscriberOf2String(name => new StubICallGateSubscriber<Action, int>()
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

        CommandManager = new()
        {
            AddHandlerStringCommandInfo = (command, commandInfo) => true,
            RemoveHandlerString = command => true,
            InstanceBehavior = StubBehaviors.NotImplemented
        };

        PluginLog = new() { InstanceBehavior = StubBehaviors.DefaultValue };
    }
}
