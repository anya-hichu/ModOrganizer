using ModOrganizer.Json.Penumbra.Manipulations.Metas.Atchs.Entries;

namespace ModOrganizer.Tests.Json.Penumbra.Manipulations.Metas.Atchs.Entries;

public static class IStubbableMetaAtchEntryReaderExtensions
{
    public static T WithMetaAtchEntryReaderTryRead<T>(this T stubbable, MetaAtchEntry? stubValue) where T : IStubbableMetaAtchEntryReader
    {
        stubbable.MetaAtchEntryReaderStub.TryReadJsonElementT0Out = (element, out instance) =>
        {
            instance = stubValue!;
            return stubValue != null;
        };

        return stubbable;
    }
}
