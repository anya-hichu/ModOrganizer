using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Groups;
using ModOrganizer.Json.Penumbra.Groups.Imcs;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Imcs.Entries;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Imcs.Identifiers;
using ModOrganizer.Json.Penumbra.Options.Imcs;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
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
            { nameof(GroupImc.Identifier), identifier },
            { nameof(GroupImc.Options), options },
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
        Assert.IsNull(groupImc.Options);
    }


    [TestMethod]
    public void TestTryReadWithInvalidKind()
    {
        var observer = new StubObserver();

        var reader = new GroupImcReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var element = JsonSerializer.SerializeToElement(null as object);

        var success = reader.TryRead(element, out var group);

        Assert.IsFalse(success);
        Assert.IsNull(group);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [Object] value kind but found [Null]: {element}", actualMessage));
    }

    [TestMethod]
    public void TestTryReadWithInvalidBaseGroup()
    {
        var observer = new StubObserver();

        var reader = new GroupImcReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithGroupBaseReaderTryRead(null)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>());

        var success = reader.TryRead(element, out var group);

        Assert.IsFalse(success);
        Assert.IsNull(group);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Debug),
            actualMessage => Assert.AreEqual($"Failed to read base [Group] for [GroupImc]: {element}", actualMessage));
    }

    [TestMethod]
    public void TestTryReadWithInvalidType()
    {
        var observer = new StubObserver();

        var type = "Invalid Type";

        var baseGroup = new Group()
        {
            Name = "Group Name",
            Type = type
        };

        var reader = new GroupImcReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithGroupBaseReaderTryRead(baseGroup)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>());

        var success = reader.TryRead(element, out var group);

        Assert.IsFalse(success);
        Assert.IsNull(group);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Failed to read [GroupImc], invalid type [{type}] (expected: Imc): {element}", actualMessage));
    }

    [TestMethod]
    [DataRow(nameof(GroupImc.AllAttributes))]
    [DataRow(nameof(GroupImc.AllVariants))]
    public void TestTryReadWithInvalidValueKind(string propertyName)
    {
        var observer = new StubObserver();

        var baseGroup = new Group()
        {
            Name = "Group Name",
            Type = GroupImcReader.TYPE
        };

        var reader = new GroupImcReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithGroupBaseReaderTryRead(baseGroup)
            .Build();

        var propertyValue = 0;

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { propertyName, propertyValue }
        });

        var success = reader.TryRead(element, out var group);

        Assert.IsFalse(success);
        Assert.IsNull(group);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [Number] value kind to be parsable as [Boolean]: {propertyValue}", actualMessage));
    }


    [TestMethod]
    public void TestTryReadWithInvalidDefaultEntry()
    {
        var observer = new StubObserver();

        var baseGroup = new Group()
        {
            Name = "Group Name",
            Type = GroupImcReader.TYPE
        };

        var reader = new GroupImcReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithGroupBaseReaderTryRead(baseGroup)
            .WithMetaImcEntryReaderTryRead(null)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(GroupImc.DefaultEntry), false }
        });

        var success = reader.TryRead(element, out var group);

        Assert.IsFalse(success);
        Assert.IsNull(group);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Failed to read [MetaImcEntry] for [GroupImc]: {element}", actualMessage));
    }

    [TestMethod]
    public void TestTryReadWithInvalidIdentifier()
    {
        var observer = new StubObserver();

        var baseGroup = new Group()
        {
            Name = "Group Name",
            Type = GroupImcReader.TYPE
        };

        var reader = new GroupImcReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithGroupBaseReaderTryRead(baseGroup)
            .WithMetaImcIdentifierReaderTryRead(null)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(GroupImc.Identifier), false }
        });

        var success = reader.TryRead(element, out var group);

        Assert.IsFalse(success);
        Assert.IsNull(group);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Failed to read [MetaImcIdentifier] for [GroupImc]: {element}", actualMessage));
    }

    [TestMethod]
    public void TestTryReadWithInvalidOptions()
    {
        var observer = new StubObserver();

        var baseGroup = new Group()
        {
            Name = "Group Name",
            Type = GroupImcReader.TYPE
        };

        var reader = new GroupImcReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithGroupBaseReaderTryRead(baseGroup)
            .WithOptionImcGenericReaderReadMany(null)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(GroupImc.Options), false }
        });

        var success = reader.TryRead(element, out var group);

        Assert.IsFalse(success);
        Assert.IsNull(group);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Failed to read one or more [OptionImc] for [GroupImc]: {element}", actualMessage));
    }
}
