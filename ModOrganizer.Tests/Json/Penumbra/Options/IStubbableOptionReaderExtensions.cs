using ModOrganizer.Json.Penumbra.Options;

namespace ModOrganizer.Tests.Json.Penumbra.Options;

public static class IStubbableOptionReaderExtensions
{
    public static T WithOptionReaderTryRead<T>(this T stubbable, Option? stubValue) where T : IStubbableOptionReader
    {
        stubbable.OptionReaderStub.TryReadJsonElementT0Out = (element, out instance) =>
        {
            instance = stubValue!;
            return stubValue != null;
        };

        return stubbable;
    }

    public static T WithOptionReaderTryReadMany<T>(this T stubbable, Option[]? stubValue) where T : IStubbableOptionReader
    {
        stubbable.OptionReaderStub.TryReadManyJsonElementT0ArrayOut = (element, out instances) =>
        {
            instances = stubValue;
            return stubValue != null;
        };

        return stubbable;
    }
}
