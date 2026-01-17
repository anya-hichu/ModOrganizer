using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Asserts;
using ModOrganizer.Json.Readers.Elements;

namespace ModOrganizer.Json.Penumbra.Groups;

public class GroupGenericReader : TypeGenericReader<Group>, IGroupGenericReader
{
    public IElementReader ElementReader { get; init; }

    public GroupGenericReader(IAssert assert, IElementReader elementReader, IReader<Group> groupCombiningReader, IReader<Group> groupImcReader, IReader<Group> groupMultiReader, 
        IReader<Group> groupSingleReader, IPluginLog pluginLog) : base(assert, pluginLog)
    {
        ElementReader = elementReader;

        Readers.Add(GroupCombiningReader.TYPE, groupCombiningReader);
        Readers.Add(GroupImcReader.TYPE, groupImcReader);
        Readers.Add(GroupMultiReader.TYPE, groupMultiReader);
        Readers.Add(GroupSingleReader.TYPE, groupSingleReader);
    }
}
