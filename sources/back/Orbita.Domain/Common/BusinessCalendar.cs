using System.Collections.Concurrent;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Orbita.Domain.Common;

/// <summary>
/// Calendar rules used by project planning: invariant month/date formats, Excel serial dates,
/// French metropolitan public holidays and working-day calculations.
/// </summary>
public static partial class BusinessCalendar
{
    private static readonly DateOnly ExcelEpoch = new(1899, 12, 30);
    private static readonly ConcurrentDictionary<int, IReadOnlyDictionary<DateOnly, string>> HolidayCache = new();

    public static string Month(DateOnly date) => date.ToString("yyyy-MM", CultureInfo.InvariantCulture);

    public static string ToIsoDate(DateOnly date) => date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    public static DateOnly ParseIsoDate(string value) =>
        DateOnly.ParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture);

    public static string ToFrenchDate(DateOnly date) => date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

    public static string ToFrenchMonth(string month)
    {
        var firstDay = ParseMonth(month);
        return firstDay.ToString("MM/yyyy", CultureInfo.InvariantCulture);
    }

    public static string AddMonths(string month, int count) =>
        Month(ParseMonth(month).AddMonths(count));

    public static int MonthsBetween(string startMonth, string endMonth)
    {
        var start = ParseMonth(startMonth);
        var end = ParseMonth(endMonth);
        return ((end.Year - start.Year) * 12) + end.Month - start.Month;
    }

    public static DateOnly FirstDayOfMonth(string month) => ParseMonth(month);

    public static DateOnly LastDayOfMonth(string month) => ParseMonth(month).AddMonths(1).AddDays(-1);

    public static IReadOnlyList<DateOnly> DaysOfMonth(string month)
    {
        var first = ParseMonth(month);
        var days = new List<DateOnly>(DateTime.DaysInMonth(first.Year, first.Month));
        for (var day = first; day.Month == first.Month; day = day.AddDays(1))
        {
            days.Add(day);
        }

        return days;
    }

    public static DateOnly FromExcelSerial(double serial)
    {
        if (!double.IsFinite(serial) || serial is < -657434 or > 2958465)
        {
            throw new ArgumentOutOfRangeException(nameof(serial));
        }

        return ExcelEpoch.AddDays(checked((int)Math.Round(serial, MidpointRounding.AwayFromZero)));
    }

    public static double ToExcelSerial(DateOnly date) => date.DayNumber - ExcelEpoch.DayNumber;

    public static DateOnly? TryParseFrenchDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var match = FrenchDate().Match(value);
        if (!match.Success ||
            !int.TryParse(match.Groups[1].Value, CultureInfo.InvariantCulture, out var day) ||
            !int.TryParse(match.Groups[2].Value, CultureInfo.InvariantCulture, out var month) ||
            !int.TryParse(match.Groups[3].Value, CultureInfo.InvariantCulture, out var year) ||
            month is < 1 or > 12 ||
            day < 1 ||
            day > DateTime.DaysInMonth(year, month))
        {
            return null;
        }

        return new DateOnly(year, month, day);
    }

    public static DateOnly EasterSunday(int year)
    {
        var a = year % 19;
        var b = year / 100;
        var c = year % 100;
        var d = b / 4;
        var e = b % 4;
        var f = (b + 8) / 25;
        var g = (b - f + 1) / 3;
        var h = (19 * a + b - d - g + 15) % 30;
        var i = c / 4;
        var k = c % 4;
        var l = (32 + 2 * e + 2 * i - h - k) % 7;
        var m = (a + 11 * h + 22 * l) / 451;
        var month = (h + l - 7 * m + 114) / 31;
        var day = ((h + l - 7 * m + 114) % 31) + 1;

        return new DateOnly(year, month, day);
    }

    public static IReadOnlyDictionary<DateOnly, string> FrenchPublicHolidays(int year) =>
        HolidayCache.GetOrAdd(year, static currentYear =>
        {
            var easter = EasterSunday(currentYear);
            return new Dictionary<DateOnly, string>
            {
                [new DateOnly(currentYear, 1, 1)] = "New Year's Day",
                [easter.AddDays(1)] = "Easter Monday",
                [new DateOnly(currentYear, 5, 1)] = "Labour Day",
                [new DateOnly(currentYear, 5, 8)] = "Victory in Europe Day",
                [easter.AddDays(39)] = "Ascension Day",
                [easter.AddDays(50)] = "Whit Monday",
                [new DateOnly(currentYear, 7, 14)] = "Bastille Day",
                [new DateOnly(currentYear, 8, 15)] = "Assumption Day",
                [new DateOnly(currentYear, 11, 1)] = "All Saints' Day",
                [new DateOnly(currentYear, 11, 11)] = "Armistice Day",
                [new DateOnly(currentYear, 12, 25)] = "Christmas Day"
            };
        });

    public static string? PublicHolidayName(DateOnly date) =>
        FrenchPublicHolidays(date.Year).TryGetValue(date, out var name) ? name : null;

    public static bool IsWorkingDay(DateOnly date) =>
        date.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday) &&
        PublicHolidayName(date) is null;

    public static int WorkingDaysBetween(DateOnly? start, DateOnly? end)
    {
        if (start is null || end is null || end < start)
        {
            return 0;
        }

        var count = 0;
        for (var date = start.Value; date <= end.Value; date = date.AddDays(1))
        {
            if (IsWorkingDay(date))
            {
                count++;
            }
        }

        return count;
    }

    private static DateOnly ParseMonth(string month)
    {
        var date = DateOnly.ParseExact(month + "-01", "yyyy-MM-dd", CultureInfo.InvariantCulture);
        return date;
    }

    [GeneratedRegex(@"(\d{1,2})/(\d{1,2})/(\d{4})", RegexOptions.CultureInvariant)]
    private static partial Regex FrenchDate();
}
