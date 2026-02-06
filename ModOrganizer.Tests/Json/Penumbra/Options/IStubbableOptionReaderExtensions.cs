using ModOrganizer.Json.Penumbra.Options;

namespace ModOrganizer.Tests.Json.Penumbra.Options;

public static class IStubbableOptionReaderExtensions
{
    public static T WithOptionReaderReadMany<T>(this T stubbable, Option[]? stubValue) where T : IStubbableOptionReader
    {
        stubbable.OptionReaderStub.TryReadManyJsonElementT0ArrayOut = (element, out instances) =>
        {
            instances = stubValue;
            return stubValue != null;
        };

        return stubbable;
    }
}
