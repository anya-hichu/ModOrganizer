using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Groups.Multis;
using ModOrganizer.Json.Penumbra.Groups.Combinings;
using ModOrganizer.Json.Penumbra.Options.Containers;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Json.Penumbra.Groups.Bases;
using ModOrganizer.Tests.Json.Penumbra.Options.Containers;
using System.Text.Json;
using ModOrganizer.Json.Penumbra.Groups;

namespace ModOrganizer.Tests.Json.Penumbra.Groups.Multis;

[TestClass]
public class TestGroupMultiReader
{
    [TestMethod]
    public void TestTryRead()
    {
        var name = "Group Name";
        var type = GroupMultiReader.TYPE;

        var baseGroup = new Group()
        {
            Name = name,
            Type = type
        };

        var options = Array.Empty<OptionContainer>();

        var reader = new GroupMultiReaderBuilder()
            .WithGroupBaseReaderTryRead(baseGroup)
            .WithOptionContainerReaderTryReadMany(options)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(GroupCombining.Options), options }
        });

        var success = reader.TryRead(element, out var group);

        Assert.IsTrue(success);
        Assert.IsNotNull(group);

        Assert.AreEqual(name, group.Name);
        Assert.AreEqual(type, group.Type);

        var groupMulti = group as GroupMulti;
        Assert.IsNotNull(groupMulti);

        Assert.AreSame(options, groupMulti.Options);
    }

    [TestMethod]
    public void TestTryReadWithDefaults()
    {
        var name = "Group Name";
        var type = GroupMultiReader.TYPE;

        var baseGroup = new Group()
        {
            Name = name,
            Type = type
        };

        var reader = new GroupMultiReaderBuilder()
            .WithGroupBaseReaderTryRead(baseGroup)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>());

        var success = reader.TryRead(element, out var group);

        Assert.IsTrue(success);
        Assert.IsNotNull(group);

        Assert.AreEqual(name, group.Name);
        Assert.AreEqual(type, group.Type);

        var groupMulti = group as GroupMulti;
        Assert.IsNotNull(groupMulti);

        Assert.IsNotNull(groupMulti.Options);
        Assert.IsEmpty(groupMulti.Options);
    }

    [TestMethod]
    public void TestTryReadWithInvalidKind()
    {
        var observer = new StubObserver();

        var reader = new GroupMultiReaderBuilder()
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

        var reader = new GroupMultiReaderBuilder()
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
            actualMessage => Assert.AreEqual($"Failed to read base [Group] for [GroupMulti]: {element}", actualMessage));
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

        var reader = new GroupMultiReaderBuilder()
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
            actualMessage => Assert.AreEqual($"Failed to read [GroupMulti], invalid type [{type}] (expected: Multi): {element}", actualMessage));
    }


    [TestMethod]
    public void TestTryReadWithInvalidOptions()
    {
        var observer = new StubObserver();

        var baseGroup = new Group()
        {
            Name = "Group Name",
            Type = GroupMultiReader.TYPE
        };

        var options = new OptionContainer[]
        { 
            new() { Name = "Option Name" } 
        };

        var reader = new GroupMultiReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithGroupBaseReaderTryRead(baseGroup)
            .WithOptionContainerReaderTryReadMany(options)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(GroupMulti.Options), options }
        });

        var success = reader.TryRead(element, out var group);

        Assert.IsFalse(success);
        Assert.IsNull(group);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected all [OptionContainer] for [GroupMulti] to have [Priority] value: {element}", actualMessage));
    }
}
