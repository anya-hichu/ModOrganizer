using Dalamud.Plugin.Services;
using ModOrganizer.Json.Penumbra.Groups.Combinings;
using ModOrganizer.Json.Penumbra.Groups.Imcs;
using ModOrganizer.Json.Penumbra.Groups.Multis;
using ModOrganizer.Json.Penumbra.Groups.Singles;
using ModOrganizer.Json.Readers;

using ModOrganizer.Json.Readers.Elements;
using ModOrganizer.Json.Readers.Generics;

namespace ModOrganizer.Json.Penumbra.Groups.Generics;

public class GroupGenericReader : GenericTypeReader<Group>, IGroupGenericReader
{
    public IElementReader ElementReader { get; init; }

    public GroupGenericReader(IElementReader elementReader, IReader<Group> groupCombiningReader, IReader<Group> groupImcReader, IReader<Group> groupMultiReader, IReader<Group> groupSingleReader, IPluginLog pluginLog) : base(pluginLog)
    {
        ElementReader = elementReader;

        Readers.Add(GroupCombiningReader.TYPE, groupCombiningReader);
        Readers.Add(GroupImcReader.TYPE, groupImcReader);
        Readers.Add(GroupMultiReader.TYPE, groupMultiReader);
        Readers.Add(GroupSingleReader.TYPE, groupSingleReader);
    }
}
