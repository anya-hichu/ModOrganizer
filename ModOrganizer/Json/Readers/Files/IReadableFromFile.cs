using Dalamud.Plugin.Services;

namespace ModOrganizer.Json.Readers.Files;

public interface IReadableFromFile<T> : IReader<T> where T : class
{
    IFileReader FileReader { get; }
    IPluginLog PluginLog { get; }
}
