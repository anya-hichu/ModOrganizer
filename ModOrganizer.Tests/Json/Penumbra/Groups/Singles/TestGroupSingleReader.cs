using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Groups;
using ModOrganizer.Json.Penumbra.Groups.Singles;
using ModOrganizer.Json.Penumbra.Options.Containers;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Json.Penumbra.Groups.Bases;
using ModOrganizer.Tests.Json.Penumbra.Options.Containers;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.Penumbra.Groups.Singles;

[TestClass]
public class TestGroupSingleReader
{
    [TestMethod]
    public void TestTryRead()
    {
        var name = "Group Name";
        var type = GroupSingleReader.TYPE;

        var baseGroup = new Group()
        {
            Name = name,
            Type = type
        };

        var options = Array.Empty<OptionContainer>();

        var reader = new GroupSingleReaderBuilder()
            .WithGroupBaseReaderTryRead(baseGroup)
            .WithOptionContainerReaderTryReadMany(options)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(GroupSingle.Options), options }
        });

        var success = reader.TryRead(element, out var group);

        Assert.IsTrue(success);
        Assert.IsNotNull(group);

        Assert.AreEqual(name, group.Name);
        Assert.AreEqual(type, group.Type);

        var groupMulti = group as GroupSingle;
        Assert.IsNotNull(groupMulti);

        Assert.AreSame(options, groupMulti.Options);
    }

    [TestMethod]
    public void TestTryReadWithDefaults()
    {
        var name = "Group Name";
        var type = GroupSingleReader.TYPE;

        var baseGroup = new Group()
        {
            Name = name,
            Type = type
        };

        var reader = new GroupSingleReaderBuilder()
            .WithGroupBaseReaderTryRead(baseGroup)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>());

        var success = reader.TryRead(element, out var group);

        Assert.IsTrue(success);
        Assert.IsNotNull(group);

        Assert.AreEqual(name, group.Name);
        Assert.AreEqual(type, group.Type);

        var groupMulti = group as GroupSingle;
        Assert.IsNotNull(groupMulti);

        Assert.IsNull(groupMulti.Options);
    }

    [TestMethod]
    public void TestTryReadWithInvalidKind()
    {
        var observer = new StubObserver();

        var reader = new GroupSingleReaderBuilder()
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

        var reader = new GroupSingleReaderBuilder()
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
            actualMessage => Assert.AreEqual($"Failed to read base [Group] for [GroupSingle]: {element}", actualMessage));
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

        var reader = new GroupSingleReaderBuilder()
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
            actualMessage => Assert.AreEqual($"Failed to read [GroupSingle], invalid type [{type}] (expected: Single): {element}", actualMessage));
    }

    [TestMethod]
    public void TestTryReadWithInvalidOptions()
    {
        var observer = new StubObserver();

        var baseGroup = new Group()
        {
            Name = "Group Name",
            Type = GroupSingleReader.TYPE
        };

        var reader = new GroupSingleReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithGroupBaseReaderTryRead(baseGroup)
            .WithOptionContainerReaderTryReadMany(null)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(GroupSingle.Options), false }
        });

        var success = reader.TryRead(element, out var group);

        Assert.IsFalse(success);
        Assert.IsNull(group);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Failed to read one or more [OptionContainer] for [GroupSingle]: {element}", actualMessage));
    }
}
