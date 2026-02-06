using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Groups;
using ModOrganizer.Json.Penumbra.Groups.Bases;
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
    public void TestTryReadWithInvalidKind()
    {
        var observer = new StubObserver();

        var reader = new GroupBaseReaderBuilder()
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
            actualMessage => Assert.AreEqual($"Failed to read [Group], unsupported [Version] found [{version}] (supported version: 0): {element}", actualMessage));
    }

    [TestMethod]
    public void TestTryReadWithEmptyName()
    {
        var observer = new StubObserver();

        var name = string.Empty;

        var reader = new GroupBaseReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(Group.Version), GroupBaseReader.SUPPORTED_VERSION },
            { nameof(Group.Name), name }
        });

        var success = reader.TryRead(element, out var group);

        Assert.IsFalse(success);
        Assert.IsNull(group);

        var calls = observer.GetCalls();
        Assert.HasCount(2, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Debug),
            actualMessage => Assert.AreEqual("Expected value to not be empty", actualMessage));

        AssertPluginLog.MatchObservedCall(calls[1], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected property [Name] value to be not empty [String]: {element}", actualMessage));
    }

    [TestMethod]
    public void TestTryReadWithEmptyType()
    {
        var observer = new StubObserver();

        var type = string.Empty;

        var reader = new GroupBaseReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(Group.Version), GroupBaseReader.SUPPORTED_VERSION },
            { nameof(Group.Name), "Name" },
            { nameof(Group.Type), type }
        });

        var success = reader.TryRead(element, out var group);

        Assert.IsFalse(success);
        Assert.IsNull(group);

        var calls = observer.GetCalls();
        Assert.HasCount(2, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Debug),
            actualMessage => Assert.AreEqual("Expected value to not be empty", actualMessage));

        AssertPluginLog.MatchObservedCall(calls[1], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected property [Type] value to be not empty [String]: {element}", actualMessage));
    }

    [TestMethod]
    [DataRow(nameof(Group.Description), JsonValueKind.String)]
    [DataRow(nameof(Group.Image), JsonValueKind.String)]
    [DataRow(nameof(Group.Page), JsonValueKind.Number)]
    [DataRow(nameof(Group.Priority), JsonValueKind.Number)]
    [DataRow(nameof(Group.DefaultSettings), JsonValueKind.Number)]
    public void TestTryReadWithInvalidValueKind(string propertyName, JsonValueKind kind)
    {
        var observer = new StubObserver();

        var reader = new GroupBaseReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .Build();

        var propertyValue = false;

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(Group.Version), GroupBaseReader.SUPPORTED_VERSION },
            { nameof(Group.Name), "Name" },
            { nameof(Group.Type), "Type" },
            { propertyName, propertyValue }
        });

        var success = reader.TryRead(element, out var group);

        Assert.IsFalse(success);
        Assert.IsNull(group);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.StartsWith($"Expected [{kind}] value kind but found [False]: {propertyValue}", actualMessage));
    }
}
