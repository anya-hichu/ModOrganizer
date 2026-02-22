using Dalamud.Plugin.Services;

namespace ModOrganizer.Json.Writers.Clipboards;

public interface IClipboardWriter<T> : IWriter<T>
{
    IPluginLog PluginLog { get; }
}
