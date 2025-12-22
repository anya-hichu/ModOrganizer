using Dalamud.Plugin.Ipc.Fakes;
using Penumbra.Api.Enums;
using Penumbra.Api.IpcSubscribers;

namespace ModOrganizer.Tests.Stubbables;

public static class IStubbablePenumbraApiExtensions
{
    public static T WithPenumbraApiGetModDirectory<T>(this T stubbable, DirectoryInfo modDirectory) where T : IStubbablePluginInterface
    {
        stubbable.PluginInterfaceStub.GetIpcSubscriberOf1String(name => new StubICallGateSubscriber<string>()
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

        return stubbable;
    }

    public static T WithPenumbraApiGetModList<T>(this T stubbable, Dictionary<string, string> modList) where T : IStubbablePluginInterface
    {
        stubbable.PluginInterfaceStub.GetIpcSubscriberOf1String(name => new StubICallGateSubscriber<Dictionary<string, string>>()
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

        return stubbable;
    }

    public static T WithPenumbraApiGetChangedItems<T>(this T stubbable, Dictionary<string, object?> changedItems) where T : IStubbablePluginInterface
    {
        stubbable.PluginInterfaceStub.GetIpcSubscriberOf3String(name => new StubICallGateSubscriber<string, string, Dictionary<string, object?>>()
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

        return stubbable;
    }

    public static T WithPenumbraApiSetModPath<T>(this T stubbable, PenumbraApiEc exitCode) where T : IStubbablePluginInterface
    {
        stubbable.PluginInterfaceStub.GetIpcSubscriberOf4String(name => new StubICallGateSubscriber<string, string, string, int>()
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

        return stubbable;
    }

    public static T WithPenumbraApiModAddedOrDeletedNoop<T>(this T stubbable) where T : IStubbablePluginInterface
    {
        stubbable.PluginInterfaceStub.GetIpcSubscriberOf2String(name => new StubICallGateSubscriber<string, object?>()
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

        return stubbable;
    }

    public static T WithPenumbraApiModMovedNoop<T>(this T stubbable) where T : IStubbablePluginInterface
    {
        stubbable.PluginInterfaceStub.GetIpcSubscriberOf3String(name => new StubICallGateSubscriber<string, string, object?>()
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

        return stubbable;
    }

    public static T WithPenumbraApiModDirectoryChangedNoop<T>(this T stubbable) where T : IStubbablePluginInterface
    {
        stubbable.PluginInterfaceStub.GetIpcSubscriberOf3String(name => new StubICallGateSubscriber<string, bool, object?>()
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

        return stubbable;
    }

    public static T WithPenumbraApiRegisterOrUnregisterSettingsSection<T>(this T stubbable, PenumbraApiEc exitCode) where T : IStubbablePluginInterface
    {
        stubbable.PluginInterfaceStub.GetIpcSubscriberOf2String(name => new StubICallGateSubscriber<Action, int>()
        {
            InvokeFuncT0 = action =>
            {
                return name switch
                {
                    RegisterSettingsSection.Label or UnregisterSettingsSection.Label => (int)exitCode,
                    _ => throw new NotImplementedException(name)
                };
            }
        });

        return stubbable;
    }
}
