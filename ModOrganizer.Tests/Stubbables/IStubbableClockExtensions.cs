namespace ModOrganizer.Tests.Stubbables;

public static class IStubbableClockExtensions
{
    public static T WithClockNewUtc<T>(this T stubbable, DateTimeOffset offset) where T : IStubbableClock
    {
        stubbable.ClockStub.GetNowUtc = () => offset;

        return stubbable;
    }
}
