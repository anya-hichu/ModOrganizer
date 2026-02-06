using ModOrganizer.Json.Penumbra.Manipulations.Metas.Imcs;

namespace ModOrganizer.Tests.Json.Penumbra.Manipulations.Metas.Imcs;

public static class IStubbableMetaImcEntryReaderExtensions
{
    public static T WithMetaImcEntryReaderTryRead<T>(this T stubbable, MetaImcEntry? stubValue) where T : IStubbableMetaImcEntryReader
    {
        stubbable.MetaImcEntryReaderStub.TryReadJsonElementT0Out = (element, out instance) =>
        {
            instance = stubValue!;
            return stubValue != null;
        };

        return stubbable;
    }
}
