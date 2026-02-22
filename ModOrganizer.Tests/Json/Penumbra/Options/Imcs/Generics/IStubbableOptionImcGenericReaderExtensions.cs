using ModOrganizer.Json.Penumbra.Options.Imcs;

namespace ModOrganizer.Tests.Json.Penumbra.Options.Imcs.Generics;

public static class IStubbableOptionImcGenericReaderExtensions
{
    public static T WithOptionImcGenericReaderTryReadMany<T>(this T stubbable, OptionImc[]? stubValue) where T : IStubbableOptionImcGenericReader
    {
        stubbable.OptionImcGenericReaderStub.TryReadManyJsonElementOptionImcArrayOut = (element, out instances) =>
        {
            instances = stubValue;
            return stubValue != null;
        };

        return stubbable;
    }
}
