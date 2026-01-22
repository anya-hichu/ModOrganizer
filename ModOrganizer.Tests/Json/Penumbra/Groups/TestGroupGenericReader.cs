using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Groups;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Json.Readers;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.Penumbra.Groups;

[TestClass]
public class TestGroupGenericReader
{
    [TestMethod]
    public void TestTryReadWithInvalidType()
    {
        var observer = new StubObserver();

        var type = "Unknown";

        var groupGenericReader = new GroupGenericReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { nameof(Group.Type), type } });

        var success = groupGenericReader.TryRead(element, out var group);

        Assert.IsFalse(success);
        Assert.IsNull(group);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Failed to get [Group] reader for type [{type}] (registered types: Combining, Imc, Multi, Single): {element}", actualMessage));
    }


    [TestMethod]
    [DataRow("Combining")]
    [DataRow("Imc")]
    [DataRow("Multi")]
    [DataRow("Single")]
    public void TestTryReadWithType(string type)
    {
        var group = new Group()
        {
            Name = string.Empty,
            Type = type
        };

        var groupGenericReader = new GroupGenericReaderBuilder()
            .WithReaderTryRead(type, group)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { nameof(Group.Type), type } });

        var success = groupGenericReader.TryRead(element, out var actualGroup);

        Assert.IsTrue(success);
        Assert.AreSame(group, actualGroup);
    }

    [TestMethod]
    [DataRow("Combining")]
    [DataRow("Imc")]
    [DataRow("Multi")]
    [DataRow("Single")]
    public void TestTryReadWithTypeWithoutSuccess(string type)
    {
        var observer = new StubObserver();

        var groupGenericReader = new GroupGenericReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithReaderTryRead(type, null as Group)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { nameof(Group.Type), type } });

        var success = groupGenericReader.TryRead(element, out var group);

        Assert.IsFalse(success);
        Assert.IsNull(group);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Debug),
            actualMessage => Assert.StartsWith($"Failed to read [Group] using reader", actualMessage));
    }
}
