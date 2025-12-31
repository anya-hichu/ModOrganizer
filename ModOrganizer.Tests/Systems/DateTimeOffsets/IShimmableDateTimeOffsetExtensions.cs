using System.Fakes;

namespace ModOrganizer.Tests.Systems.DateTimeOffsets;

public static class IShimmableDateTimeOffsetExtensions
{
    public static T WithDateTimeOffsetUtcNow<T>(this T shimmable, DateTimeOffset value) where T : IShimmableDateTimeOffset
    {
        ShimDateTimeOffset.UtcNowGet = () => value;

        return shimmable;
    }

}
