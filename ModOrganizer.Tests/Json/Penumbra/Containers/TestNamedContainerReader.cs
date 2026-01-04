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

        var jsonElement = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(NamedContainer.Name), name }
        });

        var container = new Container()
        {
            Files = [],
            FileSwaps = [],
            Manipulations = []
        };

        var namedContainerReader = new NamedContainerReaderBuilder()
            .WithContainerReaderObserver(observer)
            .WithContainerReaderTryRead(container)
            .Build();

        var success = namedContainerReader.TryRead(jsonElement, out var namedContainer);

        Assert.IsTrue(success);
        Assert.IsNotNull(namedContainer);

        Assert.AreEqual(name, namedContainer.Name);

        Assert.IsNotNull(namedContainer.Files);
        Assert.IsEmpty(namedContainer.Files);

        Assert.IsNotNull(namedContainer.FileSwaps);
        Assert.IsEmpty(namedContainer.FileSwaps);

        Assert.IsNotNull(namedContainer.Manipulations);
        Assert.IsEmpty(namedContainer.Manipulations);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        var call = calls[0];
        Assert.AreEqual(nameof(ContainerReader.TryRead), call.StubbedMethod.Name);

        switch (call.GetArguments()[0])
        {
            case JsonElement containerProperty:
                Assert.AreEqual(jsonElement, containerProperty);
                break;
            default:
                Assert.Fail("Expected first call argument to be a JsonElement");
                break;
        }
    }


    [TestMethod]
    [DataRow(0, JsonValueKind.Number)]
    [DataRow(false, JsonValueKind.False)]
    public void TestTryReadWithInvalidName(object? name, JsonValueKind kind)
    {
        var observer = new StubObserver();

        var jsonElement = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(NamedContainer.Name), name }
        });

        var namedContainerReader = new NamedContainerReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithContainerReaderTryRead(new())
            .Build();

        var success = namedContainerReader.TryRead(jsonElement, out var namedContainer);

        Assert.IsFalse(success);
        Assert.IsNull(namedContainer);

        var calls = observer.GetCalls();
        Assert.HasCount(1, calls);

        AssertPluginLog.MatchObservedCall(calls[0], nameof(IPluginLog.Warning),
            actualMessage => Assert.AreEqual($"Expected property [Name] kind for [NamedContainer] to be [String] or [Null] but found [{kind}]: {jsonElement}", actualMessage));
    }
}
