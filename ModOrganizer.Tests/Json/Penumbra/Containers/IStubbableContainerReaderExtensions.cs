using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Containers;

namespace ModOrganizer.Tests.Json.Penumbra.Containers;

public static class IStubbableContainerReaderExtensions
{
    public static T WithContainerReaderObserver<T>(this T stubbable, IStubObserver observer) where T : IStubbableContainerReader
    {
        stubbable.ContainerReaderStub.InstanceObserver = observer;

        return stubbable;
    }

    public static T WithContainerReaderTryRead<T>(this T stubbable, Container? stubValue) where T : IStubbableContainerReader
    {
        stubbable.ContainerReaderStub.TryReadJsonElementT0Out = (element, out instance) =>
        {
            instance = stubValue!;
            return stubValue != null;
        };

        return stubbable;
    }
}
