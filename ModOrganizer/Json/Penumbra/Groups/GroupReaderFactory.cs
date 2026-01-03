using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Files;

namespace ModOrganizer.Json.Penumbra.Groups;

public class GroupReaderFactory : TypeReaderFactory<Group>, IGroupReaderFactory
{
    public IFileReader FileReader { get; init; }

    public GroupReaderFactory(IReader<Group> groupCombiningReader, IReader<Group> groupImcReader, IReader<Group> groupMultiReader, IReader<Group> groupSingleReader, IFileReader fileReader, IPluginLog pluginLog) : base(pluginLog)
    {
        Readers.Add(GroupCombiningReader.TYPE, groupCombiningReader);
        Readers.Add(GroupImcReader.TYPE, groupImcReader);
        Readers.Add(GroupMultiReader.TYPE, groupMultiReader);
        Readers.Add(GroupSingleReader.TYPE, groupSingleReader);
        
        FileReader = fileReader;
    }
}
