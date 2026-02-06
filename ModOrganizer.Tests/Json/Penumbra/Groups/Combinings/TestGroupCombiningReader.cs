using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Containers;
using ModOrganizer.Json.Penumbra.Groups;
using ModOrganizer.Json.Penumbra.Groups.Combinings;
using ModOrganizer.Json.Penumbra.Options;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Json.Penumbra.Containers;
using ModOrganizer.Tests.Json.Penumbra.Groups.Bases;
using ModOrganizer.Tests.Json.Penumbra.Options;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.Penumbra.Groups.Combinings;

[TestClass]
public class TestGroupCombiningReader
{
    [TestMethod]
    public void TestTryRead()
    {
        var name = "Group Name";
        var type = GroupCombiningReader.TYPE;

        var baseGroup = new Group()
        {
            Name = name,
            Type = type
        };

        var options = Array.Empty<Option>();
        var containers = Array.Empty<NamedContainer>();

        var reader = new GroupCombiningReaderBuilder()
            .WithGroupBaseReaderTryRead(baseGroup)
            .WithNamedContainerReaderReadMany(containers)
            .WithOptionReaderReadMany(options)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(GroupCombining.Options), options },
            { nameof(GroupCombining.Containers), containers }
        });

        var success = reader.TryRead(element, out var group);

        Assert.IsTrue(success);
        Assert.IsNotNull(group);

        Assert.AreEqual(name, group.Name);
        Assert.AreEqual(type, group.Type);

        var groupCombining = group as GroupCombining;
        Assert.IsNotNull(groupCombining);

        Assert.AreSame(containers, groupCombining.Containers);
        Assert.AreSame(options, groupCombining.Options);
    }

    [TestMethod]
    public void TestTryReadWithDefaults()
    {
        var name = "Group Name";
        var type = GroupCombiningReader.TYPE;

        var baseGroup = new Group()
        {
            Name = name,
            Type = type
        };

        var reader = new GroupCombiningReaderBuilder()
            .WithGroupBaseReaderTryRead(baseGroup)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>());

        var success = reader.TryRead(element, out var group);

        Assert.IsTrue(success);
        Assert.IsNotNull(group);

        Assert.AreEqual(name, group.Name);
        Assert.AreEqual(type, group.Type);

        var groupCombining = group as GroupCombining;
        Assert.IsNotNull(groupCombining);

        Assert.IsNotNull(groupCombining.Containers);
        Assert.IsEmpty(groupCombining.Containers);

        Assert.IsNotNull(groupCombining.Options);
        Assert.IsEmpty(groupCombining.Options);
    }

    [TestMethod]
    public void TestTryReadWithInvalidKind()
    {
        var observer = new StubObserver();

        var reader = new GroupCombiningReaderBuilder()
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

        var reader = new GroupCombiningReaderBuilder()
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
            actualMessage => Assert.AreEqual($"Failed to read base [Group] for [GroupCombining]: {element}", actualMessage));
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

        var reader = new GroupCombiningReaderBuilder()
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
            actualMessage => Assert.AreEqual($"Failed to read [GroupCombining], invalid type [{type}] (expected: Combining): {element}", actualMessage));
    }

    [TestMethod]
    public void TestTryReadWithInvalidOptions()
    {
        var observer = new StubObserver();

        var baseGroup = new Group()
        {
            Name = "Group Name",
            Type = GroupCombiningReader.TYPE
        };

        var reader = new GroupCombiningReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithGroupBaseReaderTryRead(baseGroup)
            .WithOptionReaderReadMany(null)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(GroupCombining.Options), false }
        });

        var success = reader.TryRead(element, out var group);

        Assert.IsFalse(success);
        Assert.IsNull(group);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Failed to read one or more [Option] for [GroupCombining]: {element}", actualMessage));
    }

    [TestMethod]
    public void TestTryReadWithInvalidContainers()
    {
        var observer = new StubObserver();

        var baseGroup = new Group()
        {
            Name = "Group Name",
            Type = GroupCombiningReader.TYPE
        };

        var reader = new GroupCombiningReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithGroupBaseReaderTryRead(baseGroup)
            .WithNamedContainerReaderReadMany(null)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(GroupCombining.Containers), false }
        });

        var success = reader.TryRead(element, out var group);

        Assert.IsFalse(success);
        Assert.IsNull(group);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Failed to read one or more [NamedContainer] for [GroupCombining]: {element}", actualMessage));
    }
}
