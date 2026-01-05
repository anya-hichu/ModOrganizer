using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Asserts;
using ModOrganizer.Json.Readers.Elements;

namespace ModOrganizer.Json.Penumbra.Groups;

public class GroupReaderFactory : TypeReaderFactory<Group>, IGroupReaderFactory
{
    public IElementReader ElementReader { get; init; }

    public GroupReaderFactory(IAssert assert, IReader<Group> groupCombiningReader, IReader<Group> groupImcReader, IReader<Group> groupMultiReader, 
        IReader<Group> groupSingleReader, IElementReader elementReader, IPluginLog pluginLog) : base(assert, pluginLog)
    {
        Readers.Add(GroupCombiningReader.TYPE, groupCombiningReader);
        Readers.Add(GroupImcReader.TYPE, groupImcReader);
        Readers.Add(GroupMultiReader.TYPE, groupMultiReader);
        Readers.Add(GroupSingleReader.TYPE, groupSingleReader);
        
        ElementReader = elementReader;
    }
}
