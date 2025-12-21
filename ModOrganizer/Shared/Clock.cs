using System;

namespace ModOrganizer.Shared;

public class Clock : IClock
{
    public DateTimeOffset GetNowUtc() => DateTimeOffset.UtcNow;
}
