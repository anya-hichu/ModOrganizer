using Dalamud.Plugin.Services;

namespace ModOrganizer.Json.Readers.Files;

public interface IReadableFile<T> : IReader<T> where T : class
{
    FileReader FileReader { get; }
    IPluginLog PluginLog { get; }
}
