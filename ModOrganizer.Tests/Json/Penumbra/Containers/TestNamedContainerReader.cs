using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Containers;
using ModOrganizer.Json.Penumbra.Manipulations;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Json.Asserts;
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

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(NamedContainer.Name), name }
        });

        var filesOrFileSwaps = new Dictionary<string, string>();
        var manipulations = Array.Empty<ManipulationWrapper>();

        var container = new Container()
        {
            Files = filesOrFileSwaps,
            FileSwaps = filesOrFileSwaps,
            Manipulations = manipulations
        };

        var namedContainerReader = new NamedContainerReaderBuilder()
            .WithAssertIsValue(true)
            .WithAssertIsOptionalValue(name, true)
            .WithAssertIsStringDict(filesOrFileSwaps)
            .WithContainerReaderObserver(observer)
            .WithContainerReaderTryRead(container)
            .Build();

        var success = namedContainerReader.TryRead(element, out var namedContainer);

        Assert.IsTrue(success);
        Assert.IsNotNull(namedContainer);

        Assert.AreSame(name, namedContainer.Name);
        Assert.AreSame(filesOrFileSwaps, namedContainer.Files);
        Assert.AreSame(filesOrFileSwaps, namedContainer.FileSwaps);
        Assert.AreSame(manipulations, namedContainer.Manipulations);

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

        var element = JsonSerializer.SerializeToElement(new Dictionary<string, object?>()
        {
            { nameof(NamedContainer.Name), false }
        });

        var namedContainerReader = new NamedContainerReaderBuilder()
            .WithPluginLogDefaults()
            .WithPluginLogObserver(observer)
            .WithAssertIsOptionalValueSuccessfulOnTrue()
            .WithContainerReaderTryRead(new())
            .Build();

        var success = namedContainerReader.TryRead(element, out var namedContainer);

        Assert.IsFalse(success);
        Assert.IsNull(namedContainer);
    }
}
