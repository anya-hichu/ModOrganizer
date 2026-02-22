using ModOrganizer.Json.Penumbra.Options.Imcs;

namespace ModOrganizer.Tests.Json.Penumbra.Options.Imcs.AttributeMasks;

public static class IStubbableOptionImcAttributeMaskReaderExtensions
{
    public static T WithOptionImcAttributeMaskReaderTryRead<T>(this T stubbable, OptionImc? stubValue) where T : IStubbableOptionImcAttributeMaskReader
    {
        stubbable.OptionImcAttributeMaskReaderStub.TryReadJsonElementOptionImcOut = (element, out instance) =>
        {
            instance = stubValue;
            return stubValue != null;
        };

        return stubbable;
    }
}
