namespace Orbita.Domain.Common;

/// <summary>Paris calendar date independent of the host machine's configured time zone.</summary>
public static class ParisTimeZone
{
    public static TimeZoneInfo Zone { get; } = Resolve();

    public static DateOnly GetDate(DateTimeOffset instant) =>
        DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(instant, Zone).DateTime);

    private static TimeZoneInfo Resolve()
    {
        foreach (var identifier in new[] { "Europe/Paris", "Romance Standard Time" })
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(identifier);
            }
            catch (TimeZoneNotFoundException)
            {
            }
            catch (InvalidTimeZoneException)
            {
            }
        }

        return TimeZoneInfo.Utc;
    }
}
