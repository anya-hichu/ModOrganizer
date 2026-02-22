namespace ModOrganizer.Tests.Json.Readers;

public static class IStubbableReaderProviderExtensions
{
    public static S WithReaderTryRead<S, V>(this S stubbable, string type, V? stubValue) where V : class where S : IStubbableReaderProvider<V>
    {
        stubbable.GetReaderStub(type).TryReadJsonElementT0Out = (element, out instance) =>
        {
            instance = stubValue!;

            return stubValue != null;
        };

        return stubbable;
    }
}
