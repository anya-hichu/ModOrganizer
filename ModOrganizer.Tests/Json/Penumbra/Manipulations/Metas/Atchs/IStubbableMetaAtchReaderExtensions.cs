using ModOrganizer.Json.Penumbra.Manipulations.Metas.Atchs;

namespace ModOrganizer.Tests.Json.Penumbra.Manipulations.Metas.Atchs;

public static class IStubbableMetaAtchReaderExtensions
{
    public static T WithMetaAtchReaderTryRead<T>(this T stubbable, MetaAtch? stubValue) where T : IStubbableMetaAtchReader
    {
        stubbable.MetaAtchReaderStub.TryReadJsonElementT0Out = (element, out instance) =>
        {
            instance = stubValue!;
            return stubValue != null;
        };

        return stubbable;
    }
}
