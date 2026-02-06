using ModOrganizer.Json.Penumbra.Options.Imcs;

namespace ModOrganizer.Tests.Json.Penumbra.Options.Imcs;

public static class IStubbableOptionImcGenericReaderExtensions
{
    public static T WithOptionImcGenericReaderReadMany<T>(this T stubbable, OptionImc[]? stubValue) where T : IStubbableOptionImcGenericReader
    {
        stubbable.OptionImcGenericReaderStub.TryReadManyJsonElementOptionImcArrayOut = (element, out instances) =>
        {
            instances = stubValue;
            return stubValue != null;
        };

        return stubbable;
    }
}
