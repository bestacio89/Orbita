using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Orbita.Domain.Common;

/// <summary>Culture-independent text normalization used by domain matching and identity rules.</summary>
public static partial class TextNormalizer
{
    /// <summary>
    /// Removes diacritics, normalizes apostrophes, folds case and collapses whitespace.
    /// This is intended for comparison keys, never for display or persistence of source text.
    /// </summary>
    public static string Normalize(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var decomposed = value.Trim().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(decomposed.Length);

        foreach (var character in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            builder.Append(character is '’' or '‘' or '`' or '´' ? '\'' : character);
        }

        return Whitespace().Replace(builder.ToString().ToLowerInvariant(), " ").Trim();
    }

    /// <summary>Rounds like JavaScript Math.round: midpoint values are rounded toward positive infinity.</summary>
    public static decimal RoundLikeJavaScript(decimal value, int decimals = 2)
    {
        if (decimals is < 0 or > 28)
        {
            throw new ArgumentOutOfRangeException(nameof(decimals));
        }

        var factor = 1m;
        for (var index = 0; index < decimals; index++)
        {
            factor *= 10m;
        }

        return Math.Floor(value * factor + 0.5m) / factor;
    }

    [GeneratedRegex(@"\s+", RegexOptions.CultureInvariant)]
    private static partial Regex Whitespace();
}
