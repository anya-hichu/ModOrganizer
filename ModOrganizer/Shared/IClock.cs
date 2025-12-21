using System;

namespace ModOrganizer.Shared;

public interface IClock
{
    DateTimeOffset GetNowUtc();
}
