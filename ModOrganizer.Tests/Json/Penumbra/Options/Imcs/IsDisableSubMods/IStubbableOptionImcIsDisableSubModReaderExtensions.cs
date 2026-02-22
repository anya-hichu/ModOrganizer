using ModOrganizer.Json.Penumbra.Options.Imcs;

namespace ModOrganizer.Tests.Json.Penumbra.Options.Imcs.IsDisableSubMods;

public static class IStubbableOptionImcIsDisableSubModReaderExtensions
{
    public static T WithOptionImcIsDisableSubModReaderTryRead<T>(this T stubbable, OptionImc? stubValue) where T : IStubbableOptionImcIsDisableSubModReader
    {
        stubbable.OptionImcIsDisableSubModReaderStub.TryReadJsonElementOptionImcOut = (element, out instance) =>
        {
            instance = stubValue;
            return stubValue != null;
        };

        return stubbable;
    }
}
