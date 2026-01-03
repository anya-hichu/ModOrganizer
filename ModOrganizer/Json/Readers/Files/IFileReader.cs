using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers.Elements;

namespace ModOrganizer.Json.Readers.Files;

public interface IFileReader<T> : IReader<T> where T : class
{
    IElementReader ElementReader { get; }
    IPluginLog PluginLog { get; }
}
