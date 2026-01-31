using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Groups;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.Penumbra.Groups.Bases;

[TestClass]
public class TestGroupBaseReader
{
    [TestMethod]
    public void TestTryRead()
    {
        var version = 0u;
        var name = "Name";
        var description = "Description";
        var image = "Image";
        var page = 1;
        var priority = 2;
        var type = "Type";
        var defaultSettings = 3;

        var groupBaseReader = new GroupBaseReaderBuilder().Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(Group.Version), version },
            { nameof(Group.Name), name },
            { nameof(Group.Description), description },
            { nameof(Group.Image), image },
            { nameof(Group.Page), page },
            { nameof(Group.Priority), priority },
            { nameof(Group.Type), type },
            { nameof(Group.DefaultSettings), defaultSettings }
        });

        var success = groupBaseReader.TryRead(element, out var group);

        Assert.IsTrue(success);
        Assert.IsNotNull(group);

        Assert.AreEqual(version, group.Version);
        Assert.AreEqual(name, group.Name);
        Assert.AreEqual(description, group.Description);
        Assert.AreEqual(image, group.Image);
        Assert.AreEqual(page, group.Page);
        Assert.AreEqual(priority, group.Priority);
        Assert.AreEqual(type, group.Type);
        Assert.AreEqual(defaultSettings, group.DefaultSettings);
    }

    [TestMethod]
    public void TestTryReadWithInvalidVersion()
    {
        var observer = new StubObserver();

        var version = 1u;

        var groupBaseReader = new GroupBaseReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(Group.Version), version }
        });

        var success = groupBaseReader.TryRead(element, out var group);

        Assert.IsFalse(success);
        Assert.IsNull(group);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.StartsWith($"Failed to read [Group], unsupported [Version] found [{version}] (supported version: 0): {element}", actualMessage));
    }
}
