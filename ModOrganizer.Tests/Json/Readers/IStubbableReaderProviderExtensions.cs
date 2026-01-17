namespace ModOrganizer.Tests.Json.Readers;

public static class IStubbableReaderProviderExtensions
{
    public static S WithReaderTryRead<S, D>(this S stubbable, string type, D? stubValue) where D : class where S : IStubbableReaderProvider<D>
    {
        stubbable.GetReaderStub(type).TryReadJsonElementT0Out = (element, out instance) =>
        {
            instance = stubValue!;

            return stubValue != null;
        };

        return stubbable;
    }
}
