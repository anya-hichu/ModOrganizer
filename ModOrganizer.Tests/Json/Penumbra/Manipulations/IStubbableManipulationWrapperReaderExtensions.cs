using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Manipulations;

namespace ModOrganizer.Tests.Json.Penumbra.Manipulations;

public static class IStubbableManipulationWrapperReaderExtensions
{
    public static T WithManipulationWrapperReaderObserver<T>(this T stubbable, IStubObserver observer) where T : IStubbableManipulationWrapperReader
    {
        stubbable.ManipulationWrapperReaderStub.InstanceObserver = observer;

        return stubbable;
    }

    public static T WithManipulationWrapperReaderTryReadMany<T>(this T stubbable, ManipulationWrapper[]? values) where T : IStubbableManipulationWrapperReader
    {
        stubbable.ManipulationWrapperReaderStub.TryReadManyJsonElementT0ArrayOut = (element, out instances) => 
        {
            instances = values;
            return values != null;
        };

        return stubbable;
    }

    public static T WithManipulationWrapperReaderTryReadManySuccessfulOnTrue<T>(this T stubbable) where T : IStubbableManipulationWrapperReader
    {
        stubbable.ManipulationWrapperReaderStub.TryReadManyJsonElementT0ArrayOut = (element, out instances) =>
        {
            instances = element.GetBoolean() ? [] : null;
            return instances != null;
        };

        return stubbable;
    }
}
