using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers.Elements;

namespace ModOrganizer.Json.Readers.Clipboards;

public interface IClipboardReader<T> : IReader<T> where T : class
{
    IElementReader ElementReader { get; }
    IPluginLog PluginLog { get; }
}
