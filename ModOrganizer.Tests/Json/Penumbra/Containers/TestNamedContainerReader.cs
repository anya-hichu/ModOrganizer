using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Containers;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.Penumbra.Containers;

[TestClass]
public class TestNamedContainerReader
{
    [TestMethod]
    [DataRow(null)]
    [DataRow("Name")]
    public void TestTryRead(string? name)
    {
        var observer = new StubObserver();

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { nameof(NamedContainer.Name), name } });

        var namedContainerReader = new NamedContainerReaderBuilder()
            .WithContainerReaderTryRead(new())
            .WithContainerReaderObserver(observer)
            .Build();

        var success = namedContainerReader.TryRead(element, out var namedContainer);

        Assert.IsTrue(success);
        Assert.IsNotNull(namedContainer);

        Assert.AreEqual(name, namedContainer.Name);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        var call = calls[0];
        Assert.AreEqual(nameof(ContainerReader.TryRead), call.StubbedMethod.Name);

        switch (call.GetArguments()[0])
        {
            case JsonElement containerProperty:
                Assert.AreEqual(element, containerProperty);
                break;
            default:
                Assert.Fail("Expected first call argument to be a JsonElement");
                break;
        }
    }

    [TestMethod]
    public void TestTryReadWithInvalidName()
    {
        var observer = new StubObserver();

        var name = 0;
        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>() { { nameof(NamedContainer.Name), name } });

        var namedContainerReader = new NamedContainerReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithContainerReaderTryRead(new())
            .Build();

        var success = namedContainerReader.TryRead(element, out var namedContainer);

        Assert.IsFalse(success);
        Assert.IsNull(namedContainer);

        var calls = observer.GetCalls();

        Assert.HasCount(1, calls);
        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected [String] value kind but found [Number]: {name}", actualMessage));
    }
}
