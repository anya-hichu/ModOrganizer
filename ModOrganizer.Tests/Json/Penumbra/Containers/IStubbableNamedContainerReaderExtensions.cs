using ModOrganizer.Json.Penumbra.Containers;

namespace ModOrganizer.Tests.Json.Penumbra.Containers;

public static class IStubbableNamedContainerReaderExtensions
{
    public static T WithNamedContainerReaderReadMany<T>(this T stubbable, NamedContainer[]? stubValue) where T : IStubbableNamedContainerReader
    {
        stubbable.NamedContainerReaderStub.TryReadManyJsonElementT0ArrayOut = (element, out instances) =>
        {
            instances = stubValue;
            return stubValue != null;
        };

        return stubbable;
    }
}
