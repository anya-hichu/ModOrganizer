using ModOrganizer.Json.Penumbra.Groups;

namespace ModOrganizer.Tests.Json.Penumbra.Groups.Bases;

public static class IStubbableGroupBaseReaderExtensions
{
    public static T WithGroupBaseReaderTryRead<T>(this T stubbable, Group? stubValue) where T : IStubbableGroupBaseReader
    {
        stubbable.GroupBaseReaderStub.TryReadJsonElementGroupOut = (element, out instance) =>
        {
            instance = stubValue!;
            return stubValue != null;
        };

        return stubbable;
    }
}
