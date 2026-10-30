using System.Globalization;

namespace Orbita.Domain.Common;

/// <summary>Culture-explicit formatting for French business reports; never depends on server culture.</summary>
public static class FrenchNumberFormatter
{
    private static readonly NumberFormatInfo Format = new()
    {
        NumberDecimalSeparator = ",",
        NumberGroupSeparator = " ",
        CurrencyDecimalSeparator = ",",
        CurrencyGroupSeparator = " ",
        CurrencySymbol = "€",
        CurrencyPositivePattern = 3,
        CurrencyNegativePattern = 8,
        CurrencyDecimalDigits = 0
    };

    public static string PersonDays(decimal value) =>
        TextNormalizer.RoundLikeJavaScript(value).ToString("#,##0.##", Format);

    public static string Euros(decimal value) =>
        Math.Round(value, 0, MidpointRounding.AwayFromZero).ToString("C0", Format);
}
