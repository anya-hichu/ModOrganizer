using ModOrganizer.Json.Penumbra.Manipulations.Metas.Imcs.Identifiers;

namespace ModOrganizer.Tests.Json.Penumbra.Manipulations.Metas.Imcs;

public static class IStubbableMetaImcIdentifierReaderExtensions
{
    public static T WithMetaImcIdentifierReaderTryRead<T>(this T stubbable, MetaImcIdentifier? stubValue) where T : IStubbableMetaImcIdentifierReader
    {
        stubbable.MetaImcIdentifierReaderStub.TryReadJsonElementT0Out = (element, out instance) =>
        {
            instance = stubValue!;
            return stubValue != null;
        };

        return stubbable;
    }
}
