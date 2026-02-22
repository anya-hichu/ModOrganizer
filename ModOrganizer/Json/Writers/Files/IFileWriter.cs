using Dalamud.Plugin.Services;

namespace ModOrganizer.Json.Writers.Files;

public interface IFileWriter<T> : IWriter<T>
{
    IPluginLog PluginLog { get; }
}
