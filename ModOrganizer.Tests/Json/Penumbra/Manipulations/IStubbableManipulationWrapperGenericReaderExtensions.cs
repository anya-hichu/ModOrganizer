using ModOrganizer.Json.Penumbra.Manipulations;

namespace ModOrganizer.Tests.Json.Penumbra.Manipulations;

public static class IStubbableManipulationWrapperGenericReaderExtensions
{
    public static T WithManipulationWrapperGenericReaderTryReadMany<T>(this T stubbable, ManipulationWrapper[]? values) where T : IStubbableManipulationWrapperGenericReader
    {
        stubbable.ManipulationWrapperGenericReaderStub.TryReadManyJsonElementManipulationWrapperArrayOut = (element, out instances) => 
        {
            instances = values;
            return values != null;
        };

        return stubbable;
    }
}
