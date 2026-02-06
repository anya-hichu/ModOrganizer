using ModOrganizer.Json.Penumbra.Options.Containers;

namespace ModOrganizer.Tests.Json.Penumbra.Options.Containers;

public static class IStubbableOptionContainerReaderExtensions
{
    public static T WithOptionContainerReaderTryReadMany<T>(this T stubbable, OptionContainer[]? stubValue) where T : IStubbableOptionContainerReader
    {
        stubbable.OptionContainerStub.TryReadManyJsonElementT0ArrayOut = (element, out instances) =>
        {
            instances = stubValue;
            return stubValue != null;
        };

        return stubbable;
    }
}
