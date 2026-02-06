using ModOrganizer.Json.Penumbra.Manipulations.Wrappers;

namespace ModOrganizer.Tests.Json.Penumbra.Manipulations.Wrappers.Generics;

public static class IStubbableManipulationWrapperGenericReaderExtensions
{
    public static T WithManipulationWrapperGenericReaderTryReadMany<T>(this T stubbable, ManipulationWrapper[]? stubValue) where T : IStubbableManipulationWrapperGenericReader
    {
        stubbable.ManipulationWrapperGenericReaderStub.TryReadManyJsonElementManipulationWrapperArrayOut = (element, out instances) => 
        {
            instances = stubValue;
            return stubValue != null;
        };

        return stubbable;
    }
}
