using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Groups;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Json.Penumbra.Groups.Bases;
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

        var group = new Group()
        {
            Name = name,
            Type = type
        };

        var groupCombiningReader = new GroupCombiningReaderBuilder()
            .WithGroupBaseReaderTryRead(group)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>());

        var success = groupCombiningReader.TryRead(element, out var actualGroup);

        Assert.IsTrue(success);

        if (actualGroup is not GroupCombining groupCombining) 
        {
            Assert.Fail("Group is not GroupCombining");
            return;
        } 

        Assert.AreEqual(name, groupCombining.Name);
        Assert.AreEqual(type, groupCombining.Type);
        Assert.IsNull(groupCombining.DefaultSettings);

        Assert.IsNotNull(groupCombining.Options);
        Assert.IsEmpty(groupCombining.Options);

        Assert.IsNotNull(groupCombining.Containers);
        Assert.IsEmpty(groupCombining.Containers);

    }

    [TestMethod]
    public void TestTryReadWithInvalidBaseGroup()
    {
        var observer = new StubObserver();

        var groupCombiningReader = new GroupCombiningReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithGroupBaseReaderTryRead(null)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>());

        var success = groupCombiningReader.TryRead(element, out var actualGroup);

        Assert.IsFalse(success);
        Assert.IsNull(actualGroup);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Debug),
            actualMessage => Assert.AreEqual($"Failed to read base [Group] for [GroupCombining]: {element}", actualMessage));
    }

    [TestMethod]
    public void TestTryReadWithInvalidType()
    {
        var observer = new StubObserver();

        var name = "Group Name";
        var type = "Invalid Type";

        var group = new Group()
        {
            Name = name,
            Type = type
        };

        var groupCombiningReader = new GroupCombiningReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithGroupBaseReaderTryRead(group)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>());

        var success = groupCombiningReader.TryRead(element, out var actualGroup);

        Assert.IsFalse(success);
        Assert.IsNull(actualGroup);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.StartsWith($"Failed to read [GroupCombining], invalid type [{type}] (expected: Combining): {element}", actualMessage));
    }
}
