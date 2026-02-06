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

        var reader = new GroupBaseReaderBuilder().Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(Group.Version), version },
            { nameof(Group.Name), name },
            { nameof(Group.Type), type },

            { nameof(Group.Description), description },
            { nameof(Group.Image), image },
            { nameof(Group.Page), page },
            { nameof(Group.Priority), priority },
            { nameof(Group.DefaultSettings), defaultSettings }
        });

        var success = reader.TryRead(element, out var group);

        Assert.IsTrue(success);
        Assert.IsNotNull(group);

        Assert.AreEqual(version, group.Version);
        Assert.AreEqual(name, group.Name);
        Assert.AreEqual(type, group.Type);

        Assert.AreEqual(description, group.Description);
        Assert.AreEqual(image, group.Image);
        Assert.AreEqual(page, group.Page);
        Assert.AreEqual(priority, group.Priority);
        Assert.AreEqual(defaultSettings, group.DefaultSettings);
    }

    [TestMethod]
    public void TestTryReadWithDefaults()
    {
        var version = 0u;
        var name = "Name";
        var type = "Type";

        var reader = new GroupBaseReaderBuilder().Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(Group.Version), version },
            { nameof(Group.Name), name },
            { nameof(Group.Type), type }
        });

        var success = reader.TryRead(element, out var group);

        Assert.IsTrue(success);
        Assert.IsNotNull(group);

        Assert.AreEqual(version, group.Version);
        Assert.AreEqual(name, group.Name);
        Assert.AreEqual(type, group.Type);

        Assert.IsNull(group.Description);
        Assert.IsNull(group.Image);
        Assert.IsNull(group.Page);
        Assert.IsNull(group.Priority);
        Assert.IsNull(group.DefaultSettings);
    }

    [TestMethod]
    public void TestTryReadWithInvalidVersion()
    {
        var observer = new StubObserver();

        var version = 1u;

        var reader = new GroupBaseReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(Group.Version), version }
        });

        var success = reader.TryRead(element, out var group);

        Assert.IsFalse(success);
        Assert.IsNull(group);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.StartsWith($"Failed to read [Group], unsupported [Version] found [{version}] (supported version: 0): {element}", actualMessage));
    }
}
