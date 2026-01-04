using System.Fakes;

namespace ModOrganizer.Tests.Systems.DateTimeOffsets;

public static class IShimmableDateTimeOffsetExtensions
{
    public static T WithDateTimeOffsetUtcNow<T>(this T shimmable, DateTimeOffset stubValue) where T : IShimmableDateTimeOffset
    {
        shimmable.OnShimsContext += () => ShimDateTimeOffset.UtcNowGet = () => stubValue;
        return shimmable;
    }
}
