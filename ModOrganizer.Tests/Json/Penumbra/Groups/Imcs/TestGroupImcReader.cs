using ModOrganizer.Json.Penumbra.Groups;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Imcs;
using ModOrganizer.Json.Penumbra.Options.Imcs;
using ModOrganizer.Tests.Json.Penumbra.Groups.Bases;
using ModOrganizer.Tests.Json.Penumbra.Manipulations.Metas.Imcs;
using ModOrganizer.Tests.Json.Penumbra.Options.Imcs;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.Penumbra.Groups.Imcs;

[TestClass]
public class TestGroupImcReader
{
    [TestMethod]
    public void TestTryRead()
    {
        var name = "Group Name";
        var type = GroupImcReader.TYPE;

        var baseGroup = new Group()
        {
            Name = name,
            Type = type
        };

        var defaultEntry = new MetaImcEntry()
        {
            MaterialId = default,
            DecalId = default,
            VfxId = default,
            MaterialAnimationId = default,
            AttributeMask = default,
            SoundId = default
        };

        var identifier = new MetaImcIdentifier()
        {
            PrimaryId = default,
            SecondaryId = default,
            Variant = default,
            ObjectType = string.Empty,
            EquipSlot = string.Empty,
            BodySlot = string.Empty
        };

        var options = Array.Empty<OptionImc>();

        var reader = new GroupImcReaderBuilder()
            .WithGroupBaseReaderTryRead(baseGroup)
            .WithMetaImcEntryReaderTryRead(defaultEntry)
            .WithMetaImcIdentifierReaderTryRead(identifier)
            .WithOptionImcGenericReaderReadMany(options)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(GroupImc.DefaultEntry), defaultEntry },
            { nameof(GroupImc.Identifier), identifier }
        });

        var success = reader.TryRead(element, out var group);

        Assert.IsTrue(success);
        Assert.IsNotNull(group);

        Assert.AreEqual(name, group.Name);
        Assert.AreEqual(type, group.Type);

        var groupImc = group as GroupImc;
        Assert.IsNotNull(groupImc);

        Assert.AreSame(defaultEntry, groupImc.DefaultEntry);
        Assert.AreSame(identifier, groupImc.Identifier);
        Assert.AreSame(options, groupImc.Options);
    }

    [TestMethod]
    public void TestTryReadWithDefaults()
    {
        var name = "Group Name";
        var type = GroupImcReader.TYPE;

        var baseGroup = new Group()
        {
            Name = name,
            Type = type
        };

        var reader = new GroupImcReaderBuilder()
            .WithGroupBaseReaderTryRead(baseGroup)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>());

        var success = reader.TryRead(element, out var group);

        Assert.IsTrue(success);
        Assert.IsNotNull(group);

        Assert.AreEqual(name, group.Name);
        Assert.AreEqual(type, group.Type);

        var groupImc = group as GroupImc;
        Assert.IsNotNull(groupImc);

        Assert.IsNull(groupImc.AllVariants);
        Assert.IsNull(groupImc.AllAttributes);

        Assert.IsNull(groupImc.DefaultEntry);
        Assert.IsNull(groupImc.Identifier);

        Assert.IsNotNull(groupImc.Options);
        Assert.IsEmpty(groupImc.Options);
    }
}
