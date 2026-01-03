using Microsoft.QualityTools.Testing.Fakes.Stubs;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.Readers.Elements;

public static class IStubbableElementReaderExtensions
{
    public static T WithElementReaderObserver<T>(this T stubbable, IStubObserver observer) where T : IStubbableElementReader
    {
        stubbable.ElementReaderStub.InstanceObserver = observer;

        return stubbable;
    }

    public static T WithElementReaderTryReadFromFile<T>(this T stubbable, JsonElement? value) where T : IStubbableElementReader
    {
        stubbable.ElementReaderStub.TryReadFromFileStringJsonElementOut = (path, out instance) =>
        {
            instance = value.GetValueOrDefault();
            return value.HasValue;
        };

        return stubbable;
    }
}
